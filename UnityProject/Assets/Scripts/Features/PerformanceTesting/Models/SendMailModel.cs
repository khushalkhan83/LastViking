using UnityEngine;

public class SendMailModel : MonoBehaviour
{
    [SerializeField] private string _toEmail;
    public string ToEmail => _toEmail;

    public void SendEmail(string topic, string header, string message)
    {
        string subject = MyEscapeURL(topic);
        string body = MyEscapeURL(header + "\n" + message);

        Application.OpenURL("mailto:" + ToEmail + "?subject=" + subject + "&body=" + body);
    }

    private string MyEscapeURL(string url) => WWW.EscapeURL(url).Replace("+", "%20");
}
