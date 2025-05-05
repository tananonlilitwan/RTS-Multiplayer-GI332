using System;
using UnityEngine;
using TMPro;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class IPDisplay : MonoBehaviour
{
    public TextMeshProUGUI ipText; // ข้อความแสดง IP ของเครื่องตัวเอง
    public TextMeshProUGUI connectedText; // ข้อความแสดง IP ของเครื่องที่เชื่อมต่อ

    private UdpClient udpClient;
    private const int Port = 7777; // พอร์ตที่ใช้ในการสื่อสาร

    private string myIP;
    private string connectedIP;

    async void Start()
    {
        // ดึง IP ของเครื่องตัวเอง
        myIP = GetLocalIPAddress();
        ipText.text = "My IP: " + myIP;

        // เริ่ม Local Discovery เพื่อหาเครื่องที่เชื่อมต่อ
        udpClient = new UdpClient(Port);

        // เริ่มการรับข้อมูลจากเครื่องอื่น
        await StartReceiving();
    }

    // ฟังก์ชั่นที่จะดึง IP ของเครื่องตัวเอง
    string GetLocalIPAddress()
    {
        string localIP = "";
        foreach (var host in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (host.AddressFamily == AddressFamily.InterNetwork) // หา IP ที่ใช้ในเครือข่าย
            {
                localIP = host.ToString();
                break;
            }
        }
        return localIP;
    }

    // ฟังก์ชั่นที่รับข้อมูลจากเครื่องอื่น
    async Task StartReceiving()
    {
        while (true)
        {
            try
            {
                UdpReceiveResult result = await udpClient.ReceiveAsync(); // รอรับข้อมูล
                string receivedMessage = Encoding.UTF8.GetString(result.Buffer);
                
                if (receivedMessage == "DISCOVER_RTS_GAME")
                {
                    connectedIP = result.RemoteEndPoint.Address.ToString(); // เก็บ IP ของเครื่องที่เชื่อมต่อ
                    UpdateConnectedText();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("เกิดข้อผิดพลาดในการรับข้อมูล: " + ex.Message);
            }
        }
    }

    // ฟังก์ชั่นที่อัพเดตข้อความ IP ที่เชื่อมต่อ
    void UpdateConnectedText()
    {
        if (connectedText != null && !string.IsNullOrEmpty(connectedIP))
        {
            connectedText.text = "Connected to: " + connectedIP;
        }
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}
