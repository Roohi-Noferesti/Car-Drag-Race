using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (NamesConfiguration))]
public class NamesConfigurationInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Read From CSV"))
        {
            ReadCSV();
        }

        if (GUILayout.Button("Send Names to English and Persian"))
        {
            SendUsersToEnglishAndPersianNames();
        }

        base.OnInspectorGUI();
    }

    private void SendUsersToEnglishAndPersianNames()
    {
        var namesConfig = target as NamesConfiguration;


        namesConfig.EnglishNames= new List<string>();
        namesConfig.PersianArabicNames= new List<string>();
        foreach (var displayName in namesConfig.DisplayNames)
        {
            if(Regex.IsMatch(displayName, @"^[A-Za-z\d_-]+$")) namesConfig.EnglishNames.Add(displayName);
            else if(Regex.IsMatch(displayName, "^[\u0622\u0627\u0628\u067E\u062A-\u062C\u0686\u062D-\u0632\u0698\u0633-\u063A\u0641\u0642\u06A9\u06AF\u0644-\u0648\u06CC]+$")) namesConfig.PersianArabicNames.Add(displayName);
        }
    }

    private void ReadCSV()
    {
        /*var path = EditorUtility.OpenFilePanel("Select CSV", Application.dataPath, "csv");
        List<string> names = new List<string>();

        EditorUtility.DisplayProgressBar("Reading File", "Reading names from csv", 0);
        if (!string.IsNullOrEmpty(path))
        {
            using (var reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    names.Add(values[2]);
                }
            }
        }

        EditorUtility.DisplayProgressBar("Reading File", "Reading names from csv", 0.5F);*/

        string nn = "桑涵蓄 楚星火 公西祖 姚阳夏 茆天元 梅文昌 六星纬 山鸿波 艾立果 辜宏硕 令狐修杰 花高寒 漆雕新 巢天华 乜铭晨 任树 祝孟 戎锟 英乐欣 孝天睿 历舟 光日 纪宣朗 镇修然 娄飞文 栗绍晖 云永丰 释家骏 唐杰 席锐精 函洲 覃书 夷英达 丑晨涛 庚苍 圣嘉木 米兴言 建和泽 丹弘深 邴璟 蓬智伟 秋建华 包鸿畅 北乐正 肥波峻 轩辕澎湃 于武 阿鹏池 毓豫 宝永安 哀文虹 寇驰逸 后旺 晋炳君 宇文宇 路鸿羽 勤浩初 承心思 保浩广 费莫家 酒昆鹏 歧德明 原志国 安春 少力强 称晟 充良 笪兴为 范姜建同 卷博文 过夏 步嘉荣 理凯 顾鹤骞 太叔智鑫 五志义 况和光 都茂材 泣鸿轩 鲍乐湛 曲泰初 速良平 邢成和 裔建元 谢佑运 孛晋 眭浩阔 和慕 阮高爽 真玉石 项鸿哲 图门博延 隽英才 军昌茂 抄伟彦 塔理全 汗乐生 阴浩漫 荤成荫 邛俊迈 黎泽雨 委忆枫 字白易 佴觅丹 沐名 水寄云 屠春绿 贺娟娟 郭如曼 泷惠君 赫连子怀 佘双玉 象琨瑜 揭春竹 昝芳荃 亢婉丽 仇漪 戢献仪 穰丹 鹿竹筱 飞代蓝 偶代秋 莫芝宇 昂半安 籍绿凝 是从筠 许谷秋 黎初 范灵秋 汲叶农 墨绮云 镇璇玑 中婉清 阳易云 司空巧绿 汉采薇 须千亦 帛蓉 初寒天 皋平灵 剑优 罗绣梓 蓝宛亦 锁依 军嘉月 荆涵桃 赤书琴 犁雨灵 庄清怡 毛莹玉 矫山梅 海丹丹 关语海 越秀华 错虹影 於淑华 首舒扬 拱以 邴绮晴 闫从蕾 邱念烟 楼俨雅 同惜梦 万俟迎荷 丁澈 瑞幼丝 段干谷兰 姬馨欣 源迎荷 睢之卉 希丽珠 花香波 东方娅芳 张安妮 银博雅 集清馨 机翠岚 旷曼凡 友淑穆 戚月怡 定华乐 谷紫云 撒水儿 禚静雅 合思萱 壬白秋 智梦凡 聂曼吟 路晔晔 普丽华 张廖灵慧 谬丹山 都馨荣 竹秀媚 百雨晨 太叔婉容 贯友灵 慕容子芸 枚秋双 沈寒云 答安萱 文绿柳";
        var names = nn.Split(' ').ToList();

        var namesConfig = target as NamesConfiguration;
        namesConfig.DisplayNames.Clear();

        int count = 0;

        while (names.Count > 0 & names.Remove("No_Name"))
        {

        }
        while (names.Count > 0 && count<5000)
        {
            int random = Random.Range(0, names.Count);
            //namesConfig.DisplayNames.Add(names[random]);
            namesConfig.ChineseNames.Add(names[random]);
            count++;
            names.RemoveAt(random);
        }

        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("Done", "Updating names done", "Ok");
        EditorUtility.SetDirty(namesConfig);
    }
}
