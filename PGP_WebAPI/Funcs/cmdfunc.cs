namespace CmdFuncs
{
    public class Cw
    {
        private string password = "";
        private string sCommandline = "";
        private string user = "";
        private string path = "";
        private string outputpath = "";

        public void change(string company,string path,string outputpath)
        {
            this.user = company;
            this.path = path;
            this.outputpath = outputpath;
        }

        public string work(string act)
        {
            if (act == "e")
            {
                sCommandline = @"gpg -r "+user+" --output "+outputpath+" -e "+path;
            }
            else if (act == "d")
            {
                sCommandline = @"gpg --pinentry-mode loopback --passphrase "+password+" --output "+outputpath+" --decrypt "+path;
            }

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("cmd.exe");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            string curdir = preworkforpath();
            psi.WorkingDirectory = @curdir;

            System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi);

            process.StandardInput.WriteLine(sCommandline);
            process.StandardInput.Flush();
            process.StandardInput.Close();

            process.WaitForExit();

            string result = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            Console.WriteLine("error:");
            Console.WriteLine(error);
            //Console.WriteLine("result:");
            //Console.WriteLine(result);

            if(error != "")
            {
                return error;
            }

            process.Close();
            process.Dispose();

            return "success!!";
        }



        public string preworkforpath()
        {
            string temp = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            string[] arr = temp.Split('\\');
            temp = "";
            for (int i = 0; i < arr.Length - 4; i++)
            {
                temp += arr[i] + '\\';
            }

            return temp;
        }


        public Cw(string pwd)
        {
            this.password = pwd;
            this.sCommandline = "";
        }
    }
}
