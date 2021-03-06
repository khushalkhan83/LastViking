using System.Collections.Generic;
using UnityEditor;
using Extensions;

namespace CustomeEditorTools
{
    [InitializeOnLoad]
    public class EditorStartupMessages
    {
        static EditorStartupMessages()
        {
            if(EditorApplication.timeSinceStartup < 20)
            {
                EditorUtility.DisplayDialog("Приветсвие", GreatingMessages.RandomElement(),"OK");
            }
        }

        private static List<string> GreatingMessages = new List<string>()
        {
            "В лесу закапал летний дождь, и светлячки шипя погасли.",
            "Однажды я расстался с девушкой только из-за того, что она меня бросила",
            "Чем жизнь в умеренном климате отличается от тропиков и Полярного круга? В умеренном климате нужно платить и за кондиционер, и за отопление.",
            "Из зоны оцепления Чернобыльской АЭС пытался вырваться местный житель. Заградотряд открыл огонь по нарушителю. На теле задержанного солдаты насчитали 12 отверстий. К счастью, все они оказались анальными.",
            "- Доктор мне выписал это лекарство и пообещал, что с ним у меня начнется другая жизнь!- В смысле, следующая?",
            "Не считай дураком того, кто дал глупый совет. Он ведь тебе его дал, а не себе.",
            "- Мне всего год, а у меня есть свой дом, личный официант, массажист, аниматор. А ты чего достиг в этой жизни?- Ну, мне не повезло родиться котом...",
            "В магазине начальница отчитывает совсем молоденькую продавщицу за какой-то проступок и в конце своего разноса риторически восклицает:- Да у тебя вообще совесть есть?!Перепуганная девчоночка, размазывая слезы по лицу и всхлипывая, жалобно оправдывается:- Есть, я в торговле совсем недавно...",
            "Косметика из Мертвого моря: сбылась многовековая еврейская мечта делать деньги из грязи.",
            "Пессимист во всем видит трудности, оптимист - возможности, а идиот - не может отличить одно от другого.",
            "В жизни, как в электротехнике: на каждую лампочку всегда найдется свой выключатель.",
            "Сотрудник совета ветеранов - пенсионервожатый?",
            "Мечтаю о том, чтобы придумали что-то наподобие детского сада, но для взрослых. Пришел в сад, а там фуршетный стол, следом покерок, вискарик, потом сон, опять поели. Пару дней в месяц посещаешь такое учреждение и с новыми силами отправляешься на работу.",
            "- А что если Бог дал нам пандемию не в наказание, а для того, чтобы отделить важное от второстепенного? Оказалось, что без поп-звезд, фитнес-иструкторов, хождения депутатов в народ, фестивалей Нашествие жизнь возможна, а без медицины, общения с близкими, хорошего питания - нет.- Да вы просто не патриот.",
            "Вызвать у доктора интерес к своей болезни - хороший способ продления жизни.",
        };
    }
}