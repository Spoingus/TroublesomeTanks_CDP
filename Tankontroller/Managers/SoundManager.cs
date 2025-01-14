using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace Tankontroller.Managers
{
    public class SoundManager
    {
        private Dictionary<string, SoundEffect> mSoundEffects = new Dictionary<string, SoundEffect>();
        static SoundManager mInstance = new SoundManager();

        static SoundManager() { }
        private SoundManager() { }

        public static SoundManager Instance
        {
            get { return mInstance; }
        }

        public void Add(string pName)
        {
            SoundEffect soundEffect = Tankontroller.Instance().CM().Load<SoundEffect>(pName);
            mSoundEffects.Add(pName, soundEffect);
        }

        public SoundEffectInstance GetSoundEffectInstance(string pName)
        {
            return mSoundEffects[pName].CreateInstance();
        }
        public SoundEffectInstance GetLoopableSoundEffectInstance(string pName)
        {
            SoundEffectInstance instance = GetSoundEffectInstance(pName);
            instance.IsLooped = true;
            return instance;
        }
    }
}
