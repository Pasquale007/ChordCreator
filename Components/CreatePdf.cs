using System.Diagnostics;

namespace ChordCreater.Components {
    internal class CreatePdf {

        public CreatePdf(string fileName) {
            Process p1 = new Process();
            p1.StartInfo.FileName = "batch.bat";
            p1.StartInfo.Arguments = $"{SharedValues.InstanceOf.getTargetFolder()} {fileName.Split('.')[0]}";
            p1.StartInfo.UseShellExecute = false;
            p1.Start();
            p1.WaitForExit();
        }
    }
}
