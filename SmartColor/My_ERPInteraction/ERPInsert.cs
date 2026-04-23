using SmartColor.My_DataBase;
using SmartColor.My_File;
using SmartColor.My_Form.HistoricalData;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SmartColor.My_ERPInteraction
{
    internal class ERPInsert
    {
        SmartColor.My_ConPar.ERPInteraction.Txt _erp;

        struct recipe
        {
            public int _i_indexNum;
            public string _s_assistantCode;
            public string _s_assistantName;
            public double _s_formulaDosage;
            public string _s_unitOfAccount;
            public int _i_bottleNum;
            public double _d_settingConcentration;
            public double _d_realConcentration;
            public double _d_objectDropWeight;
            public string _s_code;
            public string _s_technologyName;
            public string _s_no;
            //染固色子工艺名字
            public string _s_childcode;


        }

        public ERPInsert() 
        {
            var erp = SmartColor.My_ConPar.Object.CurrentERP as SmartColor.My_ConPar.ERPInteraction.Txt;
            if (erp != null)
            {
                _erp = erp;
            }
        }

        static ReaderWriterLockSlim _logWriteLock = new ReaderWriterLockSlim();

        public async Task StartReadAsync()
        {
            try
            {
                //循环读取是否有文件
                while (true) 
                {
                    bool exists = await FileExistsWithTimeoutAsync(_erp.TxtPath, 500);
                    if (exists)
                    {
                        try
                        {
                            if (!_logWriteLock.IsWriteLockHeld)
                            {

                                _logWriteLock.EnterWriteLock(); //进入写入锁
                                string[] sa_temp = File.ReadAllLines(_erp.TxtPath);
                                File.Delete(_erp.TxtPath);
                                ReadAllLine(sa_temp);
                            }

                        }
                        catch (Exception ex)
                        {
                            if (_logWriteLock.IsWriteLockHeld)
                                _logWriteLock.ExitWriteLock(); //退出写入锁
                        }
                        finally
                        {
                            if (_logWriteLock.IsWriteLockHeld)
                                _logWriteLock.ExitWriteLock(); //退出写入锁

                        }

                    }
                    Thread.Sleep(500);
                }
            }
            catch { }

        }

        /// <summary>
        /// 检查网络共享文件是否存在（带超时，不卡顿）
        /// </summary>
        public static async Task<bool> FileExistsWithTimeoutAsync(string path, int timeoutMs = 500)
        {
            try
            {
                // 用 Task.Run 避免阻塞主线程
                var checkTask = Task.Run(() => File.Exists(path));

                // 等待任务完成 或 超时
                if (await Task.WhenAny(checkTask, Task.Delay(timeoutMs)) == checkTask)
                {
                    return checkTask.Result;
                }
                else
                {
                    // 超时 = 判定为不可访问
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }


        //arg参数用于记录所有行文件数据
        public void ReadAllLine(string[] arg)
        {
            try
            {
                string[] rcp = (string[])arg;
                int currentIndex = 0;

            label:
                string formulaCode = null;
                int versionNum = 0;
                int num = 0;
                string formulaName = null;
                double clothWeight = 0;
                double totalWeight = 0;
                double bathRatio = 0;
                double allW = 0;
                double d_non_AnhydrationWR = My_ConPar.Other.Default_Non_AnhydrationWR;
                double d_anhydrationWR = My_ConPar.Other.Default_AnhydrationWR;
                //string sOperator = null;

                //记录秤布机读取数据寄存器地址
                int i_address = 0;
                //杯号
                int i_cupNum = 0;

                for (; currentIndex < rcp.Length; currentIndex++)
                {
                    if (rcp[currentIndex].Substring(0, 4) == "500M" && rcp[currentIndex].Length == _erp.Head_Total)
                    {
                        //表头资料
                        formulaName = rcp[currentIndex].Substring(_erp.Head_FormulaName - 1, _erp.Head_FormulaName_Len).Trim();
                        num = Convert.ToInt16(rcp[currentIndex].Substring(_erp.Head_Count - 1, _erp.Head_Count_Len));
                        formulaCode = rcp[currentIndex].Substring(_erp.Head_FormulaCode - 1, _erp.Head_FormulaCode_Len).Trim();
                        clothWeight = Convert.ToDouble(rcp[currentIndex].Substring(_erp.Head_ClothWeight - 1, _erp.Head_ClothWeight_Len));
                        totalWeight = Convert.ToDouble(rcp[currentIndex].Substring(_erp.Head_TotalWeight - 1, _erp.Head_TotalWeight_Len));
                        //sOperator = rcp[currentIndex].Substring(76, 8).Trim();
                        bathRatio = Convert.ToDouble(string.Format("{0:F}", totalWeight / clothWeight));

                        //找到表头信息
                        var dt = SqlServer.Select(My_DataBase.FORMULA_HEAD.TableName,
                            $"{My_DataBase.FORMULA_HEAD.FormulaCode} = '{formulaCode}'  ORDER BY VersionNum DESC");

                        if (dt.Rows.Count>0)
                        {
                            versionNum = (Convert.ToInt16(dt.Rows[0][My_DataBase.FORMULA_HEAD.VersionNum])) + 1;
                        }
                        break;
                    }
                }


                List<recipe> list = new List<recipe>();
                for (; currentIndex < rcp.Length; currentIndex++)
                {
                    if (rcp[currentIndex].Substring(0, 4) == "500C" && rcp[currentIndex].Length == _erp.Detail_Total &&
                        rcp[currentIndex].Substring(_erp.Detail_FormulaCode - 1, _erp.Detail_FormulaCode_Len).Trim() == formulaCode)
                    {
                        if (rcp[currentIndex].Substring(_erp.Detail_AssistantCode - 1, _erp.Detail_AssistantCode_Len) != "WATER   ")
                        {
                            recipe re = new recipe();
                            re._i_indexNum = Convert.ToInt16(rcp[currentIndex].Substring(_erp.Detail_Index - 1, _erp.Detail_Index_Len));
                            re._s_assistantCode = rcp[currentIndex].Substring(_erp.Detail_AssistantCode - 1, _erp.Detail_AssistantCode_Len).Trim();
                            re._s_formulaDosage = Convert.ToDouble(rcp[currentIndex].Substring(_erp.Detail_RealConcentration - 1, _erp.Detail_RealConcentration_Len));

                            var data = SqlServer.Select(My_DataBase.ASSISTANT_DETAILS.TableName,
                            $"{My_DataBase.ASSISTANT_DETAILS.AssistantCode} = '{re._s_assistantCode}' ");

                            if (data.Rows.Count == 0)
                            {
                                throw new Exception("未找到" + re._s_assistantCode + "染助剂代码");
                            }

                            re._s_assistantName = Convert.ToString(data.Rows[0][My_DataBase.ASSISTANT_DETAILS.AssistantName]);
                            re._s_unitOfAccount = Convert.ToString(data.Rows[0][My_DataBase.ASSISTANT_DETAILS.UnitOfAccount]);

                            data = SqlServer.Select(My_DataBase.BOTTLE_DETAILS.TableName,
                            $"{My_DataBase.BOTTLE_DETAILS.AssistantCode} = '{re._s_assistantCode}' AND  RealConcentration != 0 Order BY SettingConcentration DESC;");

                            if (data.Rows.Count == 0)
                            {
                                throw new Exception("未找到" + re._s_assistantCode + "染助剂代码的瓶号");
                            }

                            
                            for (int i = 0; i < data.Rows.Count; i++)
                            {
                                double objectW = 0;
                                if (re._s_unitOfAccount == "%")
                                {
                                    objectW = Convert.ToDouble(string.Format("{0:F}",
                                        clothWeight * re._s_formulaDosage / Convert.ToDouble(data.Rows[i]["RealConcentration"])));
                                }
                                else if (re._s_unitOfAccount == "g/l")
                                {
                                    objectW = Convert.ToDouble(string.Format("{0:F}",
                                        totalWeight * re._s_formulaDosage / Convert.ToDouble(data.Rows[i]["RealConcentration"])));
                                }
                                else
                                {
                                    throw new Exception(re._s_assistantCode + "染助剂的计算单位设置异常");
                                }

                                if (objectW >= Convert.ToDouble(string.Format("{0:F}", data.Rows[i]["DropMinWeight"])))
                                {
                                    re._i_bottleNum = Convert.ToInt16(data.Rows[i]["BottleNum"]);
                                    re._d_settingConcentration = Convert.ToDouble(string.Format("{0:F}", data.Rows[i]["SettingConcentration"]));
                                    re._d_realConcentration = Convert.ToDouble(string.Format("{0:F}", data.Rows[i]["RealConcentration"]));
                                    re._d_objectDropWeight = objectW;
                                    break;
                                }
                                else
                                {
                                    if (i == data.Rows.Count - 1)
                                    {
                                        if (objectW > 0.1)
                                        {
                                            re._i_bottleNum = Convert.ToInt16(data.Rows[i]["BottleNum"]);
                                            re._d_settingConcentration = Convert.ToDouble(string.Format("{0:F}", data.Rows[i]["SettingConcentration"]));
                                            re._d_realConcentration = Convert.ToDouble(string.Format("{0:F}", data.Rows[i]["RealConcentration"]));
                                            re._d_objectDropWeight = objectW;
                                            break;
                                        }
                                        else
                                        {
                                            throw new Exception(re._s_assistantCode + "染助剂滴液量小于0.1克");
                                        }
                                    }
                                }
                            }
                            allW += re._d_objectDropWeight;
                            list.Add(re);
                        }
                        if (list.Count >= num)
                        {
                            break;
                        }
                    }
                }


                SqlServer.Insert(FORMULA_HEAD.TableName,
                            new Dictionary<string, object>
                            {
                                { FORMULA_HEAD.FormulaCode, formulaCode },
                                { FORMULA_HEAD.VersionNum,versionNum },
                                { FORMULA_HEAD.FormulaName,  formulaName},
                                { FORMULA_HEAD.AddWaterChoose,  1},
                                { FORMULA_HEAD.ClothWeight, clothWeight },
                                { FORMULA_HEAD.BathRatio, bathRatio },
                                { FORMULA_HEAD.TotalWeight, totalWeight },
                                { FORMULA_HEAD.CreateTime,  DateTime.Now} ,
                                { FORMULA_HEAD.ObjectAddWaterWeight, string.Format("{0:F}", (totalWeight - allW))},
                                { FORMULA_HEAD.Non_AnhydrationWR,  d_non_AnhydrationWR} ,
                                { FORMULA_HEAD.AnhydrationWR,  d_anhydrationWR} ,
                                { FORMULA_HEAD.Stage,  "滴液"} ,
                                { FORMULA_HEAD.HandleBathRatio,  "0"} ,
                                { FORMULA_HEAD.CupNum,  i_cupNum}
                            });

                foreach (recipe rc in list)
                {
                    SqlServer.Insert(FORMULA_DETAILS.TableName,
                            new Dictionary<string, object>
                            {
                                { FORMULA_DETAILS.FormulaCode, formulaCode },
                                { FORMULA_DETAILS.VersionNum,versionNum },
                                { FORMULA_DETAILS.IndexNum,  rc._i_indexNum},
                                { FORMULA_DETAILS.AssistantCode,  rc._s_assistantCode},
                                { FORMULA_DETAILS.FormulaDosage, rc._s_formulaDosage },
                                { FORMULA_DETAILS.UnitOfAccount, rc._s_unitOfAccount },
                                { FORMULA_DETAILS.BottleNum, rc._i_bottleNum },
                                { FORMULA_DETAILS.SettingConcentration,  rc._d_settingConcentration} ,
                                { FORMULA_DETAILS.RealConcentration, rc._d_realConcentration},
                                { FORMULA_DETAILS.AssistantName,  rc._s_assistantName} ,
                                //{ FORMULA_DETAILS.BottleSelection,  0.ToString()} ,
                                //{ FORMULA_DETAILS.ObjectPowderWeight,  0.ToString()} ,
                                //{ FORMULA_DETAILS.RealPowderWeight,  0.ToString()} ,
                                //{ FORMULA_DETAILS.RealDropWeight,  0.ToString()} ,
                                //{ FORMULA_DETAILS.IsShow,  0.ToString()} ,
                                //{ FORMULA_DETAILS.BrewingData,  DateTime.Now} ,
                                { FORMULA_DETAILS.ObjectDropWeight,  rc._d_objectDropWeight} 
                            });
                }
                

                if (currentIndex < rcp.Length - 1)
                {
                    goto label;
                }

            }
            catch (Exception ex)
            {
                //FADM_Form.CustomMessageBox.Show(ex.Message, "insert", MessageBoxButtons.OK, true);

                My_File.LocalTranslator.ShowMessage(ex.Message, "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
