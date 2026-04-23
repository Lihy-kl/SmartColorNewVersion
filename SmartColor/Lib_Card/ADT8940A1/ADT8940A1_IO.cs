namespace Lib_Card.ADT8940A1
{
    public class ADT8940A1_IO
    {
        #region 输入

        #region X轴
        /// <summary>
        /// X轴反限位
        /// </summary>
        public static int InPut_X_Reverse { get; set; } 

        /// <summary>
        /// X轴正限位
        /// </summary>
        public static int InPut_X_Corotation { get; set; }

        /// <summary>
        /// X轴原点
        /// </summary>
        public static int InPut_X_Origin { get; set; }

        /// <summary>
        /// X轴准备信号
        /// </summary>
        public static int InPut_X_Ready { get; set; }

        /// <summary>
        /// X轴报警信号
        /// </summary>
        public static int InPut_X_Alarm { get; set; }

        #endregion

        #region Y轴
        /// <summary>
        /// Y轴反限位
        /// </summary>
        public static int InPut_Y_Reverse { get; set; }

        /// <summary>
        /// Y轴正限位
        /// </summary>
        public static int InPut_Y_Corotation { get; set; }

        /// <summary>
        /// Y轴原点
        /// </summary>
        public static int InPut_Y_Origin { get; set; }

        /// <summary>
        /// Y轴准备
        /// </summary>
        public static int InPut_Y_Ready { get; set; }

        /// <summary>
        /// Y轴报警
        /// </summary>
        public static int InPut_Y_Alarm { get; set; }
        #endregion

        #region Z轴
        /// <summary>
        /// Z轴正限位
        /// </summary>
        public static int InPut_Z_Corotation { get; set; }

        /// <summary>
        /// Z轴原点
        /// </summary>
        public static int InPut_Z_Origin { get; set; }
        #endregion

        #region 上下气缸
        /// <summary>
        /// 气缸上到位信号
        /// </summary>
        public static int InPut_Cylinder_Up { get; set; }

        /// <summary>
        /// 气缸下到位信号
        /// </summary>
        public static int InPut_Cylinder_Down { get; set; }

        /// <summary>
        /// 气缸中间信号
        /// </summary>
        public static int InPut_Cylinder_Mid { get; set; }
        #endregion

        #region 抓手
        /// <summary>
        /// 抓手A松开到位信号
        /// </summary>
        public static int InPut_Tongs_A { get; set; }

        /// <summary>
        /// 抓手B松开到位信号
        /// </summary>
        public static int InPut_Tongs_B { get; set; }

        /// <summary>
        /// 针筒感应器信号
        /// </summary>
        public static int InPut_Syringe { get; set; }

        #endregion

        #region 接液盘
        /// <summary>
        /// 接液盘收回到位信号
        /// </summary>
        public static int InPut_Tray_In { get; set; }

        /// <summary>
        /// 接液盘伸出到位信号
        /// </summary>
        public static int InPut_Tray_Out { get; set; }
        #endregion

        #region 光幕
        /// <summary>
        /// 光幕A阻挡信号
        /// </summary>
        public static int InPut_Sunx_A { get; set; }

        /// <summary>
        /// 光幕B阻挡信号
        /// </summary>
        public static int InPut_Sunx_B { get; set; }
        #endregion

        #region 按钮信号
        /// <summary>
        /// 急停按钮信号
        /// </summary>
        public static int InPut_Stop { get; set; }

        #endregion


        #region 排液

        /// <summary>
        /// 排液上到位
        /// </summary>
        public static int InPut_Apocenosis_Up { get; set; }

        #endregion


        #region 泄压气缸

        /// <summary>
        /// 泄压下到位
        /// </summary>
        public static int InPut_Decompression_Down { get; set; }

        /// <summary>
        /// 泄压上到位
        /// </summary>
        public static int InPut_Decompression_Up { get; set; }

        /// <summary>
        /// 泄压下到位（右）
        /// </summary>
        public static int InPut_Decompression_Down_Right { get; set; }

        /// <summary>
        /// 泄压上到位（右）
        /// </summary>
        public static int InPut_Decompression_Up_Right { get; set; }

        #endregion


        #region 阻挡气缸

        /// <summary>
        /// 阻挡出到位
        /// </summary>
        public static int InPut_Block_Out { get; set; }

        /// <summary>
        /// 阻挡回到位
        /// </summary>
        public static int InPut_Block_In { get; set; }
        #endregion

        #region 气缸

        /// <summary>
        /// 气缸慢速中限位
        /// </summary>
        public static int InPut_Slow_Cylinder_Mid { get; set; }

        /// <summary>
        /// 气缸阻挡限位
        /// </summary>
        public static int InPut_Cylinder_Block { get; set; }
        #endregion

        #region 撑盖

        /// <summary>
        /// 撑盖开到位
        /// </summary>
        public static int InPut_SupportCover { get; set; }
        #endregion


        #region 备用输入
        /// <summary>
        /// 备用输入1
        /// </summary>
        public static int InPut_Spare_1 { get; set; }

        /// <summary>
        /// 备用输入2
        /// </summary>
        public static int InPut_Spare_2 { get; set; }

        /// <summary>
        /// 备用输入3
        /// </summary>
        public static int InPut_Spare_3 { get; set; }

       
        #endregion

        #endregion

        #region 输出

        #region X轴矢能
        /// <summary>
        /// X轴矢能
        /// </summary>
        public static int OutPut_X_Power { get; set; }
        #endregion

        #region Y轴矢能
        /// <summary>
        /// Y轴矢能
        /// </summary>
        public static int OutPut_Y_Power { get; set; }
        #endregion

        #region 上下气缸
        /// <summary>
        /// 气缸下
        /// </summary>
        public static int OutPut_Cylinder_Down { get; set; }

        /// <summary>
        /// 气缸上
        /// </summary>
        public static int OutPut_Cylinder_Up { get; set; }

        #endregion

        #region 抓手
        /// <summary>
        /// 抓手关闭
        /// </summary>
        public static int OutPut_Tongs { get; set; }

        /// <summary>
        /// 抓手关闭
        /// </summary>
        public static int OutPut_TongsOff { get; set; }

        #endregion

        #region 接液盘
        /// <summary>
        /// 接液盘伸出
        /// </summary>
        public static int OutPut_Tray { get; set; }
        #endregion

        #region 停止搅拌
        /// <summary>
        /// 停止搅拌打开
        /// </summary>
        public static int OutPut_Blender { get; set; }
        #endregion

        #region 加水
        /// <summary>
        /// 加水打开
        /// </summary>
        public static int OutPut_Water { get; set; }
        #endregion

        #region 废液回收
        /// <summary>
        /// 废液回收打开
        /// </summary>
        public static int OutPut_Waste { get; set; }
        #endregion

        #region 蜂鸣器
        /// <summary>
        /// 蜂鸣器打开
        /// </summary>
        public static int OutPut_Buzzer { get; set; }
        #endregion


        #region 排液

        /// <summary>
        /// 排液
        /// </summary>
        public static int OutPut_Apocenosis { get; set; }

        #endregion


        #region 泄压气缸
        /// <summary>
        /// 泄压气缸下
        /// </summary>
        public static int OutPut_Decompression { get; set; }

        /// <summary>
        /// 泄压气缸下（右）
        /// </summary>
        public static int OutPut_Decompression_Right { get; set; }

        #endregion

        #region 阻挡气缸

        /// <summary>
        /// 阻挡出
        /// </summary>
        public static int OutPut_Block_Out { get; set; }

        /// <summary>
        /// 阻挡回
        /// </summary>
        public static int OutPut_Block_In { get; set; }

        #endregion

        #region 气缸慢下阀

        /// <summary>
        /// 气缸慢下阀
        /// </summary>
        public static int OutPut_Slow_Cylinder { get; set; }

        #endregion

        #region 洗针

        /// <summary>
        /// 洗针进水阀
        /// </summary>
        public static int OutPut_Wash_In { get; set; }

        /// <summary>
        /// 洗针排水阀
        /// </summary>
        public static int OutPut_Wash_Out { get; set; }

        /// <summary>
        /// 洗针吹气阀
        /// </summary>
        public static int OutPut_Wash_Blow { get; set; }

        #endregion


        #region 备用输出
        /// <summary>
        /// 备用输出1
        /// </summary>
        public static int OutPut_Spare_1 { get; set; }

        /// <summary>
        /// 备用输出2
        /// </summary>
        public static int OutPut_Spare_2 { get; set; }

       

        #endregion

        #endregion

        #region 轴

        #region X轴
        /// <summary>
        /// X轴
        /// </summary>
        public static int Axis_X { get; set; }
        #endregion

        #region Y轴
        /// <summary>
        /// Y轴
        /// </summary>
        public static int Axis_Y { get; set; }
        #endregion

        #region Z轴
        /// <summary>
        /// Z轴
        /// </summary>
        public static int Axis_Z { get; set; }
        #endregion

        #region 备用轴
        /// <summary>
        /// 备用轴1
        /// </summary>
        public static int Axis_Spare_1 { get; set; }
        #endregion


        #endregion
    }
}