using UnityEngine;
using UnityEditor;
using System.IO;
using TMPro;
//[ExecuteAlways]


public class TextController : MonoBehaviour
{

    public TMP_InputField baseTxtEditor;
    public Canvas canvasD;
    public TextMeshProUGUI jsonHolder;
    private string wt = "";
    private string jTxt = "";
    private int readToIndex = 0;
    private bool bRead=false;
  
    public void WriteString()
    {
        string path = "Assets/Resources/wonder.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("Test");
        writer.Close();

        //Re-import the file to update the reference in the editor
       // AssetDatabase.ImportAsset(path);
       // TextAsset asset = Resources.Load("wonder");

        //Print the text from the file
       // Debug.Log(asset.text);
    }


    public void Start()
    {
        string path = "Assets/Resources/wonder.txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        baseTxtEditor.text = reader.ReadToEnd();
        Canvas.ForceUpdateCanvases();
        baseTxtEditor.text = baseTxtEditor.text + " ";
        reader.Close();
        CreateJSON();
    }

    public void CreateJSON()
    {
        wt = baseTxtEditor.text;
        jTxt = "";
        OutRead();
        jTxt = "{\"title\": \"" + GetAfterColon("title") + "\",\n\r";
        jTxt += "\"author\": \"" + GetAfterColon("author") + "\",\n\r";
        jTxt += "\"illustrator\": \"" + GetAfterColon("illustrator") + "\",\n\r";
        jTxt += "\"available\": 1,\n\r";
        jTxt += "\"nextInSeries\": 0,\n\r";
        jTxt += "\"pageData\": [\n\r";
        jTxt += "{\"type\": \"cover\"},\n\r";
        string narrator = GetAfterColon("narrator");

        //Start Reading Document
        for (int k = 0; k < 10; k++)
        {
            if (wt.Substring(readToIndex, 8).ToLower() == "chapter:")
            {
                OutRead();
                //Chapter
                jTxt += "{\"type\": \"chapterTitle\",\n\r";
                jTxt += "\"text\": \"" + GetAfterColon("chapter", readToIndex) + "\",\n\r";
                jTxt += "\"chapterNumber\":" + 1 + "\n\r},\n\r";
            }
            if (wt.Substring(readToIndex, 6).ToLower() == "[image")
            {
                //Image
                int number;
                if (!int.TryParse(wt.Substring(readToIndex + 7, 2), out number))
                    number = int.Parse(wt.Substring(readToIndex + 7, 1));
                jTxt += "<p><im>"+number+"<height>7<p>";
                int j = wt.IndexOf("\n", readToIndex);
                 readToIndex = j + 1;
                Debug.Log("ReadSet: " + readToIndex);

            }
            else if(wt.Substring(readToIndex, wt.IndexOf(" ", readToIndex) - readToIndex).Contains(":") ) 
            {
                InRead();
                //Dialog
                string name = wt.Substring(readToIndex, wt.IndexOf(":", readToIndex) - readToIndex);
                Debug.Log(name);
                string text =GetAfterColon(name, readToIndex);

                if (name==narrator)
                    jTxt += "<p><ch><l>"+name+"<t>"+text + "<p>";
                else
                    jTxt += "<p><ch><r>" + name + "<t>" + text+"<p>";


            }
            else
            {
                if (wt.Substring(readToIndex, 1) == "\n")
                    readToIndex++;
                else
                {
                    InRead();
                    jTxt += GrabTextLine(readToIndex);
                }
            }
        }
        OutRead();
        jTxt=jTxt.Remove(jTxt.LastIndexOf(","));
        jTxt += "\n\r]\n\r}"; //End of document

        jsonHolder.text = jTxt;
    }

    private string GetAfterColon(string name,int startIndex=0)
    {
        int i = wt.ToLower().IndexOf(name.ToLower() + ":",startIndex) + name.Length + 1; //Where name ends
        int j = wt.IndexOf("\n", i);
        if (j>readToIndex -1)
            readToIndex = j+1;
        Debug.Log("ColonSet: "+readToIndex);
        return wt.Substring(i,j-i-1).Trim();
    }

    private string GrabTextLine(int startIndex = 0)
    {
        int j = wt.IndexOf("\n", startIndex);
        if (j > readToIndex - 1)
            readToIndex = j + 1;
        Debug.Log("TextSet: " + readToIndex);
        return wt.Substring(startIndex, j - startIndex - 1).Trim();
    }

    private void InRead()
    {
        if (bRead)
            return;
        else
        {
            jTxt += "{\"type\": \"read\",\n\r" + "\"text\": \"";
            bRead = true;
        }
    }

    private void OutRead()
    {

        if (!bRead)
            return;
        else
        {
            jTxt += "\"\n\r},\n\r";
            bRead = false;
        }
    }



    public void Update()
    {

        

    }


}

