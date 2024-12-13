namespace GamePush.Data
{
    [System.Serializable]
    public enum ShareType
    {
        share,
        post,
        invite
    }

    [System.Serializable]
    public enum SocialType
    {
        vkontakte,
        odnoklassniki,
        telegram,
        twitter,
        facebook,
        moymir,
        whatsapp,
        viber
    }

    public class SocialLinks
    {
        public static string vkontakte = "https://vk.com/share.php?";
        public static string odnoklassniki = "https://connect.ok.ru/offer?";
        public static string telegram = "https://t.me/share/url?";
        public static string twitter = "https://twitter.com/share?";
        public static string facebook = "https://www.facebook.com/sharer/sharer.php?u=";
        public static string moymir = "https://connect.mail.ru/share?";
        public static string whatsapp = "https://api.whatsapp.com/send?";
        public static string viber = "viber://forward?";
    }
}
