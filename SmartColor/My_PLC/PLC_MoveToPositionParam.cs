using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>定点移动参数</summary>
    internal class PLC_MoveToPositionParam : PLC_SemiAutoParamBase
    {
        /// <summary>X轴坐标</summary>
        public ProtocolItem X { get; set; } // D801

        ///<summary>Y轴坐标</summary>
        public ProtocolItem Y { get; set; } // D803

        ///<summary>Z轴坐标</summary>
        public ProtocolItem Z { get; set; } // D805

        ///<summary>X轴速度</summary>
        public ProtocolItem XSpeed { get; set; } // D807

        ///<summary>X轴加减速</summary>
        public ProtocolItem XAcc { get; set; } // D809

        ///<summary>Y轴速度</summary>
        public ProtocolItem YSpeed { get; set; } // D811

        ///<summary>Y轴加减速</summary>
        public ProtocolItem YAcc { get; set; } // D813

        ///<summary>50g布夹是否需要回原点</summary>
        public ProtocolItem NeedHome { get; set; } // D815

        ///<summary>是否为待机位</summary>
        public ProtocolItem IsStandby { get; set; } // D816

        ///<summary>是否接液盘收回</summary>
        public ProtocolItem IsTrayIn { get; set; } //817

        ///<summary>气缸中是否允许移动</summary>
        public ProtocolItem AllowMove { get; set; } //818

        /// <summary>
        /// 定点移动参数构造函数
        /// </summary>
        /// <param name="x">X轴坐标</param>
        /// <param name="y">Y轴坐标</param>
        /// <param name="z">偏移量Z轴</param>
        /// <param name="xSpeed">X轴速度</param>
        /// <param name="xAcc">X轴加减速</param>
        /// <param name="ySpeed">Y轴速度</param>
        /// <param name="yAcc">Y轴加减速</param>
        /// <param name="needHome">50g布夹是否需要回原点</param>
        /// <param name="isStandby">是否为待机位</param>
        /// <param name="allowMove">气缸中是否允许移动</param>
        public PLC_MoveToPositionParam(int x, int y, short z, int xSpeed, int xAcc, int ySpeed, int yAcc, short needHome, int isStandby,int isTrayIn,int allowMove)
            : base(PLC.SemiAutomaticOperation.MoveToPosition)
        {
            X = new ProtocolItem(801, typeof(int), x);
            Y = new ProtocolItem(803, typeof(int), y);
            Z = new ProtocolItem(805, typeof(int), z);
            XSpeed = new ProtocolItem(807, typeof(int), xSpeed);
            XAcc = new ProtocolItem(809, typeof(int), xAcc);
            YSpeed = new ProtocolItem(811, typeof(int), ySpeed);
            YAcc = new ProtocolItem(813, typeof(int), yAcc);
            NeedHome = new ProtocolItem(815, typeof(short), needHome);
            IsStandby = new ProtocolItem(816, typeof(short), isStandby);
            IsTrayIn = new ProtocolItem(817, typeof(short), isTrayIn);
            AllowMove = new ProtocolItem(818, typeof(short), allowMove);
        }

        /// <summary>
        /// 定点移动参数构造函数
        /// </summary>
        /// <param name="x">X轴坐标</param>
        /// <param name="y">Y轴坐标</param>
        /// <param name="needHome">50g布夹是否需要回原点</param>
        /// <param name="isStandby">是否为待机位</param>
        /// <param name="allowMove">气缸中是否允许移动</param>
        public PLC_MoveToPositionParam(int x, int y, short needHome, short isStandby,short isTrayIn, int allowMove)
            : base(PLC.SemiAutomaticOperation.MoveToPosition)
        {
            var motion = My_ConPar.Object.CurrentMotion as My_ConPar.Type.PLC.Motion;

            X = new ProtocolItem(801, typeof(int), x);
            Y = new ProtocolItem(803, typeof(int), y);
            Z = new ProtocolItem(805, typeof(int), motion?.Home_Z_Offset ?? 0);
            XSpeed = new ProtocolItem(807, typeof(int), motion?.Move_X_HSpeed ?? 0);
            XAcc = new ProtocolItem(809, typeof(int), (motion?.Move_X_USpeed ?? 0));
            YSpeed = new ProtocolItem(811, typeof(int), (motion?.Move_Y_HSpeed ?? 0));
            YAcc = new ProtocolItem(813, typeof(int), (motion?.Move_Y_USpeed ?? 0));
            NeedHome = new ProtocolItem(815, typeof(short), needHome);
            IsStandby = new ProtocolItem(816, typeof(short), isStandby);
            IsTrayIn = new ProtocolItem(817, typeof(short), isTrayIn);
            AllowMove = new ProtocolItem(818, typeof(short), allowMove);
        }
    }
}
