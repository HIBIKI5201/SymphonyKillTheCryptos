using Cryptos.Runtime.Entity.Outgame.Story;
using Cryptos.Runtime.UseCase;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cryptos.Runtime
{
    public static class ScenarioActionConverter
    {
        public static IScenarioAction ActionConvert(string actionInfo, ref StringBuilder log)
        {
            // コマンド名と角括弧で囲まれた引数リストを抽出する正規表現
            // 例: Command[arg1, arg2], Command[arg1], Command[], Command
            var pattern = @"^(\w+)(?:\[([^\]]*)\])?$";
            var match = Regex.Match(actionInfo, pattern);

            if (!match.Success)
            {
                throw new Exception($"書式が不正です: {actionInfo}".ErrorString());
            }

            string command = match.Groups[1].Value;

            // 引数部分がマッチした場合のみ、カンマで分割する
            ScenarioArguments args = new(match.Groups.Count > 2 && !string.IsNullOrEmpty(match.Groups[2].Value)
                ? match.Groups[2].Value.Split(", ").Select(s => s.Trim()).ToArray()
                : Array.Empty<string>());

            try
            {
                switch (command)
                {
                    case nameof(CameraChange): return new CameraChange(args[0]);
                    case nameof(CharacterShow): return new CharacterShow(args[0], args[1], args[2]);
                    case nameof(CharacterHide): return new CharacterHide(args[0]);
                    case nameof(CharacterMove): return new CharacterMove(args[0], args[1], args[2], args[3]);
                    case nameof(CharacterRotate):return new CharacterRotate(args[0], args[1]);
                    default:
                        throw new FormatException();
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new Exception($"コマンド '{command}' に対する引数が足りていません".ErrorString());
            }
            catch (FormatException e)
            {
                throw new Exception($"不明なコマンドです: {command}".WarningString());

            }
            catch (Exception e)
            {
                throw new Exception($"コマンド '{command}' の引数処理中にエラーが発生しました。Args: [{args.ToString()}]\n{e}".ErrorString());
            }
        }

        public static string ErrorString(this string text) => $"<color=red>{text}</color>";
        public static string WarningString(this string text) => $"<color=yellow>{text}</color>";
    }
}
