using System.IO;
using System.IO.Compression;
using System.Text.Json;

My_Copy M = new();
M.CreateSettings();
M.LoadSettings();
M.CreateFolder();
M.CopyFiles();
M.ArchiveFiles();

Console.WriteLine(M.Log);
Console.WriteLine("SourcePath: " + M.pathSource + "\nTargetPath: " + M.pathTarget);

Console.WriteLine("\nPress any key to continue.");
Console.Read();
M.Del();


class My_Copy
{
    string? reserv = "\\Reserv";
    public string? Log { get; set; }
    public string? pathSource { get; set; }
    public string? pathTarget { get; set; }
    public async void CreateSettings()
    {
        try
        {
            using (FileStream f = new("Settings_Folder.json", FileMode.Create))
            {
                Settings_Folder folder = new("D:\\Test", "D:\\Test\\TempTEST");
                await JsonSerializer.SerializeAsync<Settings_Folder>(f, folder);
                //Console.WriteLine("Файл создан.\n");
            }
        }
        catch (Exception e) { Console.WriteLine(e.Message); }
    }
    public void LoadSettings()
    {
        try
        {
            using (FileStream f2 = new("Settings_Folder.json", FileMode.Open))
            {
                Settings_Folder? folder = JsonSerializer.Deserialize<Settings_Folder>(f2);
                pathSource = folder.Source;
                pathTarget = folder.Target;
            }
        }
        catch (Exception e) { Console.WriteLine(e.Message); }       
    }
    public void CreateFolder()
    {
        Directory.CreateDirectory(pathSource + reserv);
        Directory.CreateDirectory(pathSource + "\\Log");
        File.Create(pathSource + "\\Log\\Log.txt");  // из-за этого создания файла крашится или просто не записывает
    }
    public void ArchiveFiles()
    {
        Directory.CreateDirectory(pathTarget);
        File.Delete(pathTarget + "\\Archive.zip");
        try
        {       
            ZipFile.CreateFromDirectory(pathSource + reserv, pathTarget + "\\Archive.zip");
        }
        catch (Exception e) { Console.WriteLine(e.Message); }
    }
    public void CopyFiles()
    {
        try
        {
            foreach (var fi in new DirectoryInfo(pathSource).EnumerateFiles("*.*", SearchOption.TopDirectoryOnly))
            {
                if (fi.Exists)
                {
                    File.Copy(fi.FullName, pathSource + reserv + "\\" + fi.Name, true);
                    Log += fi.FullName + '\n';
                }
            }
        }
        catch (Exception e) { Console.WriteLine(e.Message); }
        SaveLog();
    }
    public async void SaveLog()
    {        
        try
        {
            if (File.Exists(pathSource + "\\Log\\Log.txt"))
                await File.AppendAllTextAsync(pathSource + "\\Log\\Log.txt", Log); //запись в файл
        }
        catch(Exception e) { Console.WriteLine(e.Message); }
    }
    public void Del()
    {
        Directory.Delete(pathSource + reserv, true);
    }
    ~My_Copy()
    {
        Console.WriteLine("Destructor");
        Del();
    }
}

class Settings_Folder
{
    public string Source { get; set; }
    public string Target { get; set; }
    public Settings_Folder(string source, string target)
    {
        Source = source;
        Target = target;
    }
}