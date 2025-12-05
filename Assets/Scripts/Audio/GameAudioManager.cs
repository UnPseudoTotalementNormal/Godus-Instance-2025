using System.Collections.Generic;
using System.Linq;
using Extensions;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Assertions;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace AudioSystem
{
    public class GameAudioManager : MonoBehaviour
    {
        public static GameAudioManager instance;
        
        private Dictionary<string, EventInstance> eventInstances = new();
        
        private List<EventInstance> activeMusicInstances = new();

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void StopMusic(string _eventPath)
        {
            foreach (var _m in activeMusicInstances.ToList())
            {
                _m.getDescription(out EventDescription _desc);
                _desc.getPath(out string _path);
                
                if (_path == _eventPath)
                {
                    _m.stop(STOP_MODE.ALLOWFADEOUT);
                    _m.release();
                    activeMusicInstances.Remove(_m);
                }
            }
            
            if (activeMusicInstances.Count > 0)
            {
                activeMusicInstances[^1].start();
            }
        }
        
        public void PlayMusic(string _eventPath)
        {
            if (string.IsNullOrEmpty(_eventPath))
            {
                return;
            }
            
            EventReference _eventReference = RuntimeManager.PathToEventReference(_eventPath);
            EventInstance _newMusicInstance = RuntimeManager.CreateInstance(_eventReference);

            Assert.IsTrue(_newMusicInstance.isValid(), "the new music instance is not valid with path: " + _eventPath);
            
            if (activeMusicInstances.Count != 0)
            {
                activeMusicInstances[^1].stop(STOP_MODE.ALLOWFADEOUT);
            }
            
            _newMusicInstance.start();
            activeMusicInstances.Add(_newMusicInstance);
        }

        public void PlayOneShot(EventReference _eventReference)
        {
            PlayOneShot(_eventReference.GetPath());
        }
        public void PlayOneShot(string _eventPath)
        {
            if (string.IsNullOrEmpty(_eventPath))
            {
                return;
            }
            
            EventReference _eventReference = RuntimeManager.PathToEventReference(_eventPath);
            _eventReference.TryPlayOneShot();
        }
        
        public void PlayEventInstance(EventReference _eventReference, string _instanceKey)
        {
            PlayEventInstance(_eventReference.GetPath(), _instanceKey);
        }
        
        public void PlayEventInstance(string _eventPath, string _instanceKey)
        {
            if (string.IsNullOrEmpty(_eventPath))
            {
                return;
            }
            
            EventReference _eventReference = RuntimeManager.PathToEventReference(_eventPath);
            EventInstance _instance = RuntimeManager.CreateInstance(_eventReference);
            _instance.start();

            if (eventInstances.ContainsKey(_instanceKey))
            {
                StopEventInstance(_instanceKey);
            }
                
            eventInstances.Add(_instanceKey, _instance);
        }
        

        public void StopEventInstance(string _instanceKey)
        {
            if (string.IsNullOrEmpty(_instanceKey))
            {
                return;
            }
            
            if (eventInstances.TryGetValue(_instanceKey, out EventInstance _instance))
            {
                if (_instance.isValid())
                {
                    _instance.stop(STOP_MODE.ALLOWFADEOUT);
                    _instance.release();
                }
                eventInstances.Remove(_instanceKey);
            }
        }

        private void OnDestroy()
        {
            foreach (var _music in activeMusicInstances)
            {
                if (_music.isValid())
                {
                    _music.stop(STOP_MODE.ALLOWFADEOUT);
                    _music.release();
                }
            }
            activeMusicInstances.Clear();
            
            foreach (var _eventInstance in eventInstances.Values)
            {
                if (_eventInstance.isValid())
                {
                    _eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
                    _eventInstance.release();
                }
            }
            eventInstances.Clear();
        }
    }
}
