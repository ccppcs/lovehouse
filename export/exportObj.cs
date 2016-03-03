using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;

public class exportObj
{

    private static exportObj eobj = null;
    public static exportObj Eobj
    {
        get
        {
            if (eobj == null)
            {
                eobj = new exportObj();
            }
            return eobj;
        }
    }

    public string[] exobj = new string[50];
    public int count = 0;
    public int objCount = 0;
    public Vector3 pos;
    //public List<objStatus> saveObj = new List<objStatus>();
    public SaveFile save = new SaveFile();
    public SaveFile tempsave = new SaveFile();
    public string savePath;
    public string tempString;
    public Quaternion tempQuaternion;

    private exportObj()
    {
        savePath = null;
        if (Application.platform == RuntimePlatform.Android)
        {
            savePath = "mnt/sdcard/Unity/Save/";
        }
        else
        {
            savePath = "D:/Save/";
        }
        if (!System.IO.Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);
    }

    //저장한 파일 불러오기
    public void loadObjectFile()
    {
        //SavePath는 세이브 파일이 저장된 경로
        using (Stream st = new FileStream(savePath + "save.sav", FileMode.Open))
        {

            BinaryFormatter binFormatter = new BinaryFormatter();
            SaveFile savefile = binFormatter.Deserialize(st) as SaveFile;
            for (int i = 0; i < savefile.List.Count; i++)
            {

                objStatus tempObjStatus = new objStatus();


                GameObject[] gobj = ObjReader.use.ConvertFile(savefile.List[i].objPath, true);
                gobj[0].transform.position = new Vector3(savefile.List[i].vx, savefile.List[i].vy, savefile.List[i].vz);
                gobj[0].transform.rotation = new Quaternion(savefile.List[i].qx, savefile.List[i].qy, savefile.List[i].qz, savefile.List[i].qw);
                gobj[0].gameObject.AddComponent<BoxCollider>();
                gobj[0].gameObject.AddComponent<MouseDrag_Object>();
                gobj[0].AddComponent<Rigidbody>();
                gobj[0].GetComponent<Rigidbody>().useGravity = true;
                gobj[0].GetComponent<Rigidbody>().angularDrag = 5000f;
                gobj[0].GetComponent<Rigidbody>().drag = 1f;
                gobj[0].GetComponent<Rigidbody>().mass = 1000f;
                gobj[0].GetComponent<Rigidbody>().freezeRotation = true;
                gobj[0].tag = "Furniture";
                gobj[0].name = i.ToString();

                tempObjStatus.objPath = savefile.List[i].objPath;
                tempObjStatus.objName = i.ToString();
                tempObjStatus.vx = savefile.List[i].vx;
                tempObjStatus.vy = savefile.List[i].vy;
                tempObjStatus.vz = savefile.List[i].vz;
                tempObjStatus.qx = savefile.List[i].qx;
                tempObjStatus.qy = savefile.List[i].qy;
                tempObjStatus.qz = savefile.List[i].qz;
                tempObjStatus.qw = savefile.List[i].qw;
                save.List.Add(tempObjStatus);
                objCount++;
            }
        }
    }

    //초기화

    public void Ini()
    {
        tempsave.List.Clear();
        for (int k = 0; k < save.List.Count; k++)
        {
            if (save.List[k].objPath == null)
                continue;


            objStatus tempObjStatus = new objStatus();

            Transform tempT = GameObject.Find(k.ToString()).GetComponent<Transform>();
            tempObjStatus.objPath = save.List[k].objPath;
            tempObjStatus.vx = tempT.position.x;
            tempObjStatus.vy = tempT.position.y;
            tempObjStatus.vz = tempT.position.z;
            tempObjStatus.qx = tempT.rotation.x;
            tempObjStatus.qy = tempT.rotation.y;
            tempObjStatus.qz = tempT.rotation.z;
            tempObjStatus.qw = tempT.rotation.w;
            tempsave.List.Add(tempObjStatus);
            /*
            Debug.Log(k+ " x = "+save.List[k].vx);
            Debug.Log(k+ " y = "+save.List[k].vy);
            Debug.Log(k+ " z = "+save.List[k].vz);
            */
        }
    }

    //파일저장
    public void fileSave()
    {
        Ini();

        using (Stream st = new FileStream(savePath + "save.sav", FileMode.Create))
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            binFormatter.Serialize(st, tempsave);
            st.Close();
        }
    }

    //파일삭제
    public void deleteObject(string index)
    {
        int temp = int.Parse(index);
        save.List[temp].objPath = null;
    }




}
[System.Serializable]
public class objStatus
{
    public string objPath;
    public string objName;
    public float vx;
    public float vy;
    public float vz;
    public float qx;
    public float qy;
    public float qz;
    public float qw;
}

[System.Serializable]
public class SaveFile
{
    public List<objStatus> List = new List<objStatus>();
}

