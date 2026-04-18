using SmartColor.My_ConPar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_File
{
    public static  class LocalTranslator
    {
        private static  readonly Dictionary<string, string> ZhEnDict = new Dictionary<string, string>
        {
            { "保存成功!", "Saved successfully!" },
            { "请完善所有资料后再点存档", "Please complete all the information before clicking on the archive" },
            { "温馨提示", "Tips" },
            { "错误", "Error" },
            { "提示", "Info" },
            { "请选择要删除的染助剂!", "Please select the dyeing assistant to be deleted first!" },
            { "确认删除此染助剂吗?", "Are you sure you want to delete this dyeing assistant?" },
            { "是", "Yes" },
            { "否", "No" }
            // ... 继续扩展 ...
        };

        /// <summary>
        /// 根据当前语言设置翻译文本
        /// </summary>
        /// <param name="zh">中文文本</param>
        /// <returns>翻译后的文本</returns>
        public static  string ZhToEn(string zh)
        {
            if (string.IsNullOrWhiteSpace(zh))
            {
                Logger.Info("翻译文本为空或仅包含空白字符，直接返回原始值。");
                return zh;
            }

            // 根据 Machine.Language 的值判断返回语言
            if (Machine.Language == 1) // 1 表示英文
            {
                if (ZhEnDict.TryGetValue(zh, out var translated))
                {
                    Logger.Info($"翻译文本: 原始='{zh}', 翻译='{translated}'");
                    return translated;
                }

                Logger.Info($"翻译文本: 原始='{zh}', 未找到对应的英文翻译，返回原始值。");
                return zh;
            }

         
            return zh; // 默认返回中文
        }

        /// <summary>
        /// 显示消息框，并记录日志
        /// </summary>
        /// <param name="text">消息内容</param>
        /// <param name="caption">标题</param>
        /// <param name="buttons">按钮类型</param>
        /// <param name="icon">图标类型</param>
        public static  DialogResult ShowMessage(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            var translatedMessage = ZhToEn(text);
            var translatedCaption = ZhToEn(caption);

            Logger.Info($"显示消息框: 标题='{translatedCaption}', 内容='{translatedMessage}', 按钮='{buttons}', 图标='{icon}'");
            return System.Windows.Forms.MessageBox.Show(translatedMessage, translatedCaption, buttons, icon);
        }

     
    }
}