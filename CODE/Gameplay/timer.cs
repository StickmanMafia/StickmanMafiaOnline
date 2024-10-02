using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class timer : MonoBehaviour
{
    private const string DefaultNtpServer = "pool.ntp.org";
    private const int NtpPort = 123;
    private const int NtpPacketSize = 48;
    private const byte NtpLeapIndicator = 0;
    private const byte NtpVersionNumber = 4;
    private const byte NtpMode = 3;

    public DateTime endTime;
    private TimeSpan remainingTime;
    public Text text;
    public string caseId;
    public CaseBehaivour behaivour;
    public DateTime CurrentTime { get; private set; }
    private TimeSpan timeDifference;
    private Image parentParentImage;
    public Color ColorA = Color.red; // Цвет, когда кейс не открывается
    public Color ColorB = Color.yellow; // Цвет, когда кейс открывается
    public Color ColorC = Color.green; // Цвет, когда кейс открыт
    public CaseOpenGem gemsOpen;

    private void Start()
    {
        parentParentImage = transform.parent.parent.GetComponent<Image>();
    }

    public void LoadMe()
    {
        
        if(!PlayerPrefs.HasKey("EndTime_" + caseId)){
            
            if(PlayerPrefs.GetString("Time_" + caseId)=="")
                Debug.Log(caseId +" не найден!!!");

            endTime = DateTime.Now.AddMinutes(int.Parse(PlayerPrefs.GetString("Time_" + caseId)));
           
             remainingTime = endTime - (DateTime.Now + timeDifference);
              if (remainingTime.TotalSeconds <= 0)
            {
                text.text = "";
                behaivour.CanOpen = true;
                behaivour.Opened();
                gemsOpen.gameObject.SetActive(false);
                UpdateParentParentImageColor(ColorC);
            }
            else{
                gemsOpen.gameObject.SetActive(true);
                int gems = int.Parse(PlayerPrefs.GetString("Time_" + caseId)) * 60;
                gemsOpen.CalcucatePrice(gems);
            }
        }
        else{
             endTime = DateTime.Parse(PlayerPrefs.GetString("EndTime_" + caseId));
              remainingTime = endTime - (DateTime.Now + timeDifference);
              if (remainingTime.TotalSeconds <= 0)
            {
                text.text = "";
                behaivour.CanOpen = true;
                behaivour.Opened();
                gemsOpen.gameObject.SetActive(false);
                UpdateParentParentImageColor(ColorC);
            }
            else{
                gemsOpen.gameObject.SetActive(true);
                int gems = int.Parse(PlayerPrefs.GetString("Time_" + caseId)) * 60;
                gemsOpen.CalcucatePrice(gems);
            }
        }
        
        
    }

    public IEnumerator StartSharmankaCoroutine()
    {
        yield return StartCoroutine(GetInternetTime());
        caseId = behaivour.id;
        if (PlayerPrefs.HasKey("EndTime_" + caseId))
        {
            endTime = DateTime.Parse(PlayerPrefs.GetString("EndTime_" + caseId));
        }
        else
        {
            endTime = DateTime.Now.AddMinutes(int.Parse(PlayerPrefs.GetString("Time_" + caseId)));
            PlayerPrefs.SetString("EndTime_" + caseId, endTime.ToString());
        }
    }

    public void SetTimer(float time, string caseId)
    {
        endTime = DateTime.Now.AddMinutes(time);
        PlayerPrefs.SetString("EndTime_" + caseId, endTime.ToString());
        this.caseId = caseId;
    }

    public void UpdateParentParentImageColor(Color color)
    {
        if (parentParentImage != null)
        {
            parentParentImage.color = color;
        }
    }
    public void Check(){
           if(!PlayerPrefs.HasKey("EndTime_" + caseId)){
            Debug.Log("ищем id " +caseId);
            endTime = DateTime.Now.AddMinutes(int.Parse(PlayerPrefs.GetString("Time_" + caseId)));
           
             remainingTime = endTime - (DateTime.Now + timeDifference);
              if (remainingTime.TotalSeconds <= 0)
            {
                
                UpdateParentParentImageColor(ColorC);
            }
            else{
                UpdateParentParentImageColor(ColorB);
            }
        }
        else{
             endTime = DateTime.Parse(PlayerPrefs.GetString("EndTime_" + caseId));
              remainingTime = endTime - (DateTime.Now + timeDifference);
              if (remainingTime.TotalSeconds <= 0)
            {
                
                UpdateParentParentImageColor(ColorC);
            }
            else{
                 UpdateParentParentImageColor(ColorA);
            }
        }
    }
    void Update()
    {

        if (PlayerPrefs.HasKey("EndTime_" + caseId) && endTime != DateTime.MinValue)
        {

            remainingTime = endTime - (DateTime.Now + timeDifference);
            if (remainingTime.TotalSeconds <= 0)
            {
                text.text = "";
                behaivour.CanOpen = true;
                behaivour.Opened();
                gemsOpen.gameObject.SetActive(false);
                UpdateParentParentImageColor(ColorC);
            }
            else
            {
                gemsOpen.gameObject.SetActive(true);
                text.text = FormatTimeSpan(remainingTime);
                gemsOpen.CalcucatePrice(remainingTime);
                UpdateParentParentImageColor(ColorB);
            }
        }
        else
        {
            text.text = "";
            UpdateParentParentImageColor(ColorA);
        }
    }

    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        return string.Format("{0:D2}:{1:D2}:{2:D2}",
            timeSpan.Hours,
            timeSpan.Minutes,
            timeSpan.Seconds);
    }

    private IEnumerator GetInternetTime()
    {
        DateTime internetTime = GetNetworkTime();
        CurrentTime = internetTime;
        timeDifference = internetTime - DateTime.Now;
        yield return null;
    }

    private DateTime GetNetworkTime()
    {
        var ntpData = new byte[NtpPacketSize];
        ntpData[0] = NtpLeapIndicator << 6 | NtpVersionNumber << 3 | NtpMode;

        var addresses = Dns.GetHostEntry(DefaultNtpServer).AddressList;
        var ipEndPoint = new IPEndPoint(addresses[0], NtpPort);
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        socket.Connect(ipEndPoint);
        socket.ReceiveTimeout = 3000;

        socket.Send(ntpData);
        socket.Receive(ntpData);
        socket.Close();

        ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
        ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

        var milliseconds = (intPart * 1000) + (fractPart * 1000 / 0x100000000L);
        var networkDateTime = new DateTime(1900, 1, 1).AddMilliseconds((long)milliseconds);

        // Применяем смещение часового пояса
        TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
        TimeSpan offset = localTimeZone.GetUtcOffset(networkDateTime);
        networkDateTime = networkDateTime.Add(offset);

        return networkDateTime;
    }
}
