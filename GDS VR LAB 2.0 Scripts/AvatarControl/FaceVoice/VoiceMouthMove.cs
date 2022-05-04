#if NORMCORE

using UnityEngine;

namespace Normal.Realtime.Examples {
    public class VoiceMouthMove : MonoBehaviour {

        public SkinnedMeshRenderer[] skinned;
        private RealtimeAvatarVoice _voice;
        private float _mouthSize;

        void Awake() {
            _voice = GetComponent<RealtimeAvatarVoice>();
        }

        void Update() {
            if (skinned != null) //not sure whether work or not
            {
                float targetMouthSize = Mathf.Lerp(0.1f, 1.0f, _voice.voiceVolume);
              
                _mouthSize = Mathf.Lerp(_mouthSize, targetMouthSize, 30.0f * Time.deltaTime);

              
                foreach (SkinnedMeshRenderer s in skinned)//not sure whether work or not
                {
                    s.SetBlendShapeWeight(41, _mouthSize * 100);
                    s.SetBlendShapeWeight(17, _mouthSize * 50);
                    s.SetBlendShapeWeight(6, _mouthSize * 20);
                }



            }
        }
    }
}

#endif
