namespace sqlHash
{
    class CryptoHelper
    {
        public static string HashStringMD5(string givenString)
        {
            // shamelessly borrowed from https://stackoverflow.com/questions/3984138/hash-string-in-c-sharp
            // and https://stackoverflow.com/questions/11454004/calculate-a-md5-hash-from-a-string
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                //   return string.IsNullOrEmpty(givenString) ? string.Empty : BitConverter.ToString(md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(givenString))).Replace("-", string.Empty);
                return string.IsNullOrEmpty(givenString) ? string.Empty : System.Convert.ToBase64String(md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(givenString))).Replace("=","");
                //return (string.IsNullOrEmpty(givenString) ? string.Empty : BitConverter.ToString(md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(givenString))).Replace("-", string.Empty) ) + " " +
                // ( string.IsNullOrEmpty(givenString) ? string.Empty : System.Convert.ToBase64String(md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(givenString))).Replace("=","")) ;
            }
        }
        //note: since this is a database utility and NOT a password file - MD5 is more than adequate - a collision is still unlikely and no/little negative consequences if so.        
        //      I mean if an adversary has modified your database structure in such as way as to have MD5 hash collide and you can't detect the structural difference
        //      a. They are WAY smarter than you
        //      b. They are WAY more resourced than you
        //      c. Go into hiding - they're crazy and/or extremely motivated and a show of ability/strength like that is only a harbinger of bad things/resources/etc they could throw at you if you resist.
        //      In fact - if this actually happened, they must like you, and gave you a subtle hint to "stay the heck out of our way - we could have just scrubbed you"
        //      and you should act accordingly. LOL
    }
}
