using System.Collections.Generic;
//============================================================
namespace FKLib
{
    public class INetworkUser
    {
        public string UserID { get; private set; }      // User's unique ID
        public string UserName { get; private set; }    // User's name
        public bool IsHost {  get; private set; }       // Is this user the host of the room
        public Dictionary<string, string> CustomProperties { get; private set; }    // User's custom properties

        public INetworkUser(string userName, string userID, Dictionary<string, string> customProperties, bool isHost = false)
        {
            UserName = userName;
            UserID = userID;
            CustomProperties = customProperties;
            IsHost = isHost;
        }

        public override bool Equals(object obj)
        {
            return obj is INetworkUser && (obj as INetworkUser).UserID.Equals(UserID);
        }

        public override int GetHashCode()
        {
            return UserID.GetHashCode();
        }
    }
}
