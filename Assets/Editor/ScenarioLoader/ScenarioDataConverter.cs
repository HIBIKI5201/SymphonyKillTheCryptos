using Cryptos.Runtime.Entity.Outgame.Story;
using Cryptos.Runtime.InfraStructure.OutGame.Story;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Cryptos.Runtime
{
    public static class ScenarioDataConverter
    {
        public static ScenarioDataAsset Execute(string text, ref StringBuilder log)
        {
            List<ScenarioNode> textDatas = new();
            StringReader reader = new StringReader(text);

            int lineNumber = 0;
            string pendingLine = string.Empty;

            while (reader.Peek() != -1)
            {
                string rawLine = reader.ReadLine();

                if (pendingLine.Length > 0)
                    pendingLine += "\n" + rawLine;
                else
                    pendingLine = rawLine;

                // クォート数を確認
                if (!IsQuoteClosed(pendingLine))
                {
                    // まだ閉じていないので次行へ
                    continue;
                }

                string[] elements = SplitCsvLine(pendingLine);
                pendingLine = string.Empty;

                if (elements.Length < 3)
                {
                    log.AppendLine($"要素が足りません\n".ErrorString());
                    break;
                }

                if (!bool.TryParse(elements[2], out bool isWaitForInput))
                {
                    isWaitForInput = true;
                    log.AppendLine($"line{lineNumber + 1}: bool値のパースに失敗\n".WarningString());
                }

                Span<string> actions = new(elements, 3, elements.Length - 3);
                List<IScenarioAction> actiondata = new(actions.Length);

                for (int i = 0; i < actions.Length; i++)
                {
                    if (string.IsNullOrEmpty(actions[i])) continue;

                    try
                    {
                        var action = ScenarioActionConverter.ActionConvert(actions[i], ref log);
                        if (action != null) actiondata.Add(action);
                    }
                    catch (Exception e)
                    {
                        log.AppendLine($"line{lineNumber + 1}, index {i}: {e.Message}");
                    }
                }

                ScenarioNode node = new(elements[1], elements[0], isWaitForInput, actiondata.ToArray());
                textDatas.Add(node);

                lineNumber++;
            }

            ScenarioDataAsset data = ScriptableObject.CreateInstance<ScenarioDataAsset>();
            data.Initialize(textDatas.ToArray());
            return data;
        }

        private static bool IsQuoteClosed(string line)
        {
            int quoteCount = 0;
            foreach (char c in line)
            {
                if (c == '"') quoteCount++;
            }

            // 偶数なら閉じている
            return quoteCount % 2 == 0;
        }

        private static string[] SplitCsvLine(string line)
        {
            // カンマの後に偶数個の引用符が続く場所でのみカンマで分割する正規表現
            string pattern = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";
            string[] values = Regex.Split(line, pattern);

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = values[i].Trim().Trim('"');
            }

            return values;
        }
    }
}
