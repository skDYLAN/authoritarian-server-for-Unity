using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TabPanel : MonoBehaviour {

    class playerData // параметры для сохранения
    {
        // пользовательские (можно изменять)
        public NetworkInstanceId id;
        public string nickName;
        public float ping;

        public RectTransform rectTransNick;
        public Text textNick;

        public RectTransform rectTransPing;
        public Text textPing;

    }

    Transform _transform;
    List<playerData> listPlayerData = new List<playerData>();
    public Font _font;

    public void DisplayTanPanel(NetworkInstanceId[] id, string[] nickName, float[] ping)
    {

        for (int i = 0; i < listPlayerData.Count; i++)
        {
            bool listIsFound = false;
            for (int j=0; j< id.Length; j++)
            {
                if (listPlayerData[i].id == id[j])
                {
                    listIsFound = true;
                    break;
                }
            }
            if (listIsFound == false)
            {
                listPlayerData.RemoveAt(i);
                if (listPlayerData.Count > 0)
                    i--;
            }
        }

        for (int i = 0; i < id.Length; i++)
        {
            int index = 0;
            bool listIsFound = false;

            for (int j = 0; j < listPlayerData.Count; j++)
            {
                if (listPlayerData[j].id == id[i])
                {
                    index = j;
                    listIsFound = true;
                    break;
                }
                listIsFound = false;
            }
            if (listIsFound == false)
            {
                playerData pData = new playerData();
                pData.id = id[i];
                pData.nickName = nickName[i];
                pData.ping = ping[i];

                listPlayerData.Add(pData);

                GameObject objNick = new GameObject();
                objNick.GetComponent<Transform>().SetParent(GetComponent<Transform>());
                RectTransform rectTransNick = objNick.AddComponent<RectTransform>();
                Text textNick = objNick.AddComponent<Text>();
                textNick.font = _font;

                rectTransNick.anchorMax = new Vector2(0, 1);
                rectTransNick.anchorMin = new Vector2(0, 1);
                rectTransNick.pivot = new Vector2(0, 1);
                rectTransNick.anchoredPosition3D = new Vector3(25f, -30f + (-20 * listPlayerData.Count), 0f);

                textNick.text = nickName[i];

                GameObject objPing = new GameObject();
                objPing.GetComponent<Transform>().SetParent(GetComponent<Transform>());
                RectTransform rectTransPing = objPing.AddComponent<RectTransform>();
                Text textPing = objPing.AddComponent<Text>();
                textPing.font = _font;

                rectTransPing.anchorMax = new Vector2(0, 1);
                rectTransPing.anchorMin = new Vector2(0, 1);
                rectTransPing.pivot = new Vector2(0, 1);

                rectTransPing.anchoredPosition3D = new Vector3(590f, -30f + (-20 * listPlayerData.Count), 0f);

                textPing.text = Mathf.Ceil(ping[i]*1000).ToString();

                listPlayerData[listPlayerData.Count - 1].rectTransNick = rectTransNick;
                listPlayerData[listPlayerData.Count - 1].textNick = textNick;
                listPlayerData[listPlayerData.Count - 1].rectTransPing = rectTransPing;
                listPlayerData[listPlayerData.Count - 1].textPing = textPing;
            }
            else
            {
                if(listPlayerData[index].nickName != nickName[i])
                {
                    listPlayerData[index].nickName = nickName[i];
                    listPlayerData[index].textNick.text = nickName[i];
                }

                if (listPlayerData[index].ping != ping[i])
                {
                    listPlayerData[index].ping = ping[i];
                    listPlayerData[index].textPing.text = Mathf.Ceil(ping[i] * 1000).ToString();
                }
            }

        }

        for (int i = 0; i < listPlayerData.Count; i++)
        {
            listPlayerData[i].rectTransNick.anchoredPosition3D = new Vector3(25f, -30f + (-20 * i), 0f);
            listPlayerData[i].rectTransPing.anchoredPosition3D = new Vector3(590f, -30f + (-20 * i), 0f);
        }

        _transform = GetComponent<Transform>();
    }
   
}
