using System;
using System.Collections.Generic;
//============================================================
namespace FKLib
{
    [Serializable]
    public class StaticUnitGeneratorData
    {
        public List<string> RandomNeutrallyUnitPrefabPaths = new List<string>();
        public int NeutrallyUnitNumber; // ս����������ɵ�����Unit����
        public int ProtectedColumns;    // ��������ǰ����������ǰ���в���ˢ�³�����Unit��Ŀ����ʹ����ս�����������У�
    }
}
