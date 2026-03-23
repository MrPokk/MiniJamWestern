//using BitterECS.Core;
//using Cysharp.Threading.Tasks;

//public class HandUpdateSystem : IHandSucceedRemove, IHandSucceedExtraction, IHandStackSucceedAdd
//{
//    public Priority Priority => Priority.High;

//    public UniTask ResultStackSucceedAdd(HandStackControllerDice stack)
//    {
//        return OnAddHand((HandControllerDice)stack.hand);
//    }

//    public UniTask ResultSucceedExtraction(HandControllerDice hand)
//    {
//        return OnAddHand(hand);
//    }

//    public UniTask ResultSucceedRemove(HandControllerDice hand)
//    {
//        return OnAddHand(hand);
//    }

//    private static UniTask OnAddHand(HandControllerDice hand)
//    {
//        var currentCount = hand.Items.Count;
//        var max = hand.maxCountDice;

//        var countToDraw = max - currentCount;

//        for (var i = 0; i < countToDraw; i++)
//        {
//            if (!hand.handStackController.DrawToHand())
//            {
//                break;
//            }
//        }
//        return UniTask.CompletedTask;
//    }
//}
