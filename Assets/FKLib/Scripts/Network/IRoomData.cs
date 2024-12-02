using System.Collections.Generic;
//============================================================
namespace FKLib
{
    public class IRoomData
    {
        public INetworkUser LocalUser { get; private set; }             // The local user's network data
        public IEnumerable<INetworkUser> Users { get; private set; }    // All the users in the room
        public int UserCount { get; private set; }                      // The users' number in the room
        public int MaxUsers { get; private set; }                       // The max number of users allowed in the room
        public string RoomName { get; private set; }                    // This room's name
        public string RoomID { get; private set; }                      // This room's unique ID

        public IRoomData(INetworkUser localUser, IEnumerable<INetworkUser> users, int userCount, int maxUsers, string roomName, string roomID)
        {
            LocalUser = localUser;
            Users = users;
            UserCount = userCount;
            MaxUsers = maxUsers;
            RoomName = roomName;
            RoomID = roomID;
        }
    }
}
