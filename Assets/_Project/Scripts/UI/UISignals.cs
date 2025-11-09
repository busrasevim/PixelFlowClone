using UnityEngine;

namespace _Project.Scripts.UI
{
   public readonly struct OnPlaySignal{}

   public readonly struct OnRestartSignal
   {
      public readonly bool Hard;

      public OnRestartSignal(bool hard)
      {
         Hard = hard;
      }
      
   }
   
   
   public readonly struct OnLevelSetupRequestedSignal
   {
         
   }
}
