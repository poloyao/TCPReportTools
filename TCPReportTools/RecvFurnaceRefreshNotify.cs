using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPReportTools
{
    public struct RecvFurnaceRefreshNotify
    {
        public ushort MsgCode; //
        public ushort MsgLength;

        public int RefreshFlag;  //刷新标志位

        //public int RefreshChargingFlag; //进炉刷新标志位
        //public int RefreshDischargingFlag;  //出炉刷新标志位
        //public int RefreshStepFlag; //步距刷新标志位

        public float TempPreheat1;    //预热段温度轧侧上
        public float TempPreheat2;    //预热段温度轧侧下
        public float TempPreheat3;    //预热段温度非轧侧上
        public float TempPreheat4;    //预热段温度非轧侧下

        public float TempHeat11;    //加热一段温度轧侧上
        public float TempHeat12;    //加热一段温度轧侧下
        public float TempHeat13;    //加热一段温度非轧侧上
        public float TempHeat14;    //加热一段温度非轧侧下

        public float TempHeat21;    //加热二段轧侧上
        public float TempHeat22;    //加热二段轧侧下
        public float TempHeat23;    //加热二段非轧侧上
        public float TempHeat24;    //加热二段非轧侧下

        public float TempHold1;     //均热段轧侧上
        public float TempHold2;     //均热段轧侧下
        public float TempHold3;     //均热段非轧侧上
        public float TempHold4;     //均热段非轧侧下

        public float Temp2HeatHigh1;
        public float Temp2HeatHigh2;

        public float TempHoldHigh1;
        public float TempHoldHigh2;
    }
}