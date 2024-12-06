using System.Collections.Generic;
//============================================================
namespace FKLib
{
    public interface IJsonSerializable
    {
        void GetObjectData(Dictionary<string, object> data);
        void SetObjectData(Dictionary<string, object> data);
    }
}
