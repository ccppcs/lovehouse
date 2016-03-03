using UnityEngine;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;


public class exportString : MonoBehaviour
{
    public bool threadFlag = false;
    public string savePath;
    public Quaternion tempQuaternion;

    // Use this for initialization
    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            savePath = "mnt/sdcard/Unity/Save/";
        }
        else
        {
            savePath = "D:/";
        }
        if (!System.IO.Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);
        threadFlag = false;
        Debug.Log("Create");
        ThreadStart th = new ThreadStart(work); //1.work메소드를 위임.
        Thread t = new Thread(th); //2.쓰레드생성.
        t.Start();
    }

    // Update is called once per frame
    public void work()
    {


        int maxv = 0, maxvn = 0, maxvt = 0;
        int vncount = 0;
        int vtcount = 0;
        int vcount = 0;
        string temp = "";
        string temp2 = "";

        temp += "mtllib save.mtl\r\n";
        for (int i = 0; i < exportObj.Eobj.tempsave.List.Count; i++)
        {

            string file = File.ReadAllText(exportObj.Eobj.tempsave.List[i].objPath);



            int index = 0;
            var lineInfo = new string[0];
            string[] fileLines = file.Split('\n');
            Debug.Log(fileLines.Length);
            while (index < fileLines.Length)
            {
                var line = fileLines[index++];
                if (line.Length < 3 || line[0] == '#') continue;
                char char1 = System.Char.ToLower(line[0]);
                char char2 = System.Char.ToLower(line[1]);
                if (char1 == 'm' && char2 == 't')
                {
                    continue;
                }
                if (char1 == 'v' && char2 == ' ')
                {
                    vcount++;
                    lineInfo = line.Split(' ');
                    temp += "v ";
                    float t;
                    t = float.Parse(lineInfo[1], CultureInfo.InvariantCulture) + exportObj.Eobj.tempsave.List[i].vx;
                    temp += t.ToString() + " ";
                    t = float.Parse(lineInfo[2], CultureInfo.InvariantCulture) + exportObj.Eobj.tempsave.List[i].vy;
                    temp += t.ToString() + " ";
                    t = float.Parse(lineInfo[3], CultureInfo.InvariantCulture) + exportObj.Eobj.tempsave.List[i].vz;
                    temp += t.ToString() + "\r\n";
                }
                else if (char1 == 'v' && char2 == 't')
                {
                    vtcount++;
                    temp += line + "\n";
                }
                else if (char1 == 'v' && char2 == 'n')
                {
                    vncount++;
                    temp += line + "\n";
                }
                else if (char1 == 'f' && char2 == ' ')
                {
                    lineInfo = line.Split(' ');
                    temp += "f ";
                    int t;
                    for (int j = 1; j < lineInfo.Length; j++)
                    {
                        var lineInfoParts = lineInfo[j].Split('/');
                        t = int.Parse(lineInfoParts[0]) + maxv;
                        lineInfoParts[0] = t.ToString();
                        if (lineInfoParts.Length > 1)
                        {
                            if (lineInfoParts[1] != "")
                            {
                                t = int.Parse(lineInfoParts[1]) + maxvt;
                                lineInfoParts[1] = t.ToString();
                            }
                            if (lineInfoParts.Length == 3)
                            {
                                t = int.Parse(lineInfoParts[2]) + maxvn;
                                lineInfoParts[2] = t.ToString();
                            }
                        }
                        temp += System.String.Join("/", lineInfoParts) + " ";
                    }
                    temp += "\r\n";


                }
                else
                {
                    temp += line + "\n";
                }

            }



            maxv += vcount;
            maxvn += vncount;
            maxvt += vtcount;
            vcount = 0;
            vncount = 0;
            vtcount = 0;
        }
        //obj파일로 저장
        FileStream file2 = new FileStream(savePath + "save.obj", FileMode.OpenOrCreate, FileAccess.ReadWrite);
        StreamWriter sw = new StreamWriter(file2);
        sw.WriteLine(temp);
        sw.Close();
        file2.Close();

        for (int i = 0; i < exportObj.Eobj.tempsave.List.Count; i++)
        {
            string file = exportObj.Eobj.tempsave.List[i].objPath;
            file = file.Replace(".obj", ".mtl");
            Debug.Log(file);
            temp2 += File.ReadAllText(file);
            Debug.Log(temp2);
            temp2 += "\r\n";
        }

        FileStream file4 = new FileStream(savePath + "save.mtl", FileMode.OpenOrCreate, FileAccess.ReadWrite);
        StreamWriter sw2 = new StreamWriter(file4);
        sw2.WriteLine(temp2);
        sw2.Close();
        file4.Close();

        Debug.Log("끝");






    }

}
