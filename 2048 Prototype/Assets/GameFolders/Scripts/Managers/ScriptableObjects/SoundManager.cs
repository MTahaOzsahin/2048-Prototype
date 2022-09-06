using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Scripts.Managers.ScriptableObjects
{
    [CreateAssetMenu(menuName ="Managers/SoundsManager")]
    public class SoundManager : ScriptableObject
    {
        [Tooltip("The sound for when moving.")]
        [SerializeField] AudioClip moveSound;
        [SerializeField, Range(0f, 1f)] float moveSoundVolume;

        [Tooltip("The sound for when merge blocks.")]
        [SerializeField] AudioClip mergingSounds;
        [SerializeField, Range(0f, 1f)] float mergingSoundsVolume;

        [Tooltip("the sound for when nice score earned like 512 or 1024")]
        [SerializeField] AudioClip niceScoreSound;
        [SerializeField,Range(0f, 1f)] float niceScoreSoundVolume;

        [Tooltip("The sound for when win.")]
        [SerializeField] AudioClip winSound;
        [SerializeField, Range(0f, 1f)] float winSoundVolume;

        [Tooltip("The sound for when lose.")]
        [SerializeField] AudioClip loseSound;
        [SerializeField, Range(0f, 1f)] float loseSoundvolume;

        public void MoveSound(AudioSource audioSource)
        {
            audioSource.PlayOneShot(moveSound, moveSoundVolume);
        }
        public void MergingSound(AudioSource audioSource)
        {
            audioSource.PlayOneShot(mergingSounds, mergingSoundsVolume);
        }
        public void NiceScoreSound(AudioSource audioSource)
        {
            audioSource.PlayOneShot(niceScoreSound, niceScoreSoundVolume);
        }
        public void WinSound(AudioSource audioSource)
        {
            audioSource.PlayOneShot(winSound, winSoundVolume);
        }
        public void LoseSound(AudioSource audioSource)
        {
            audioSource.PlayOneShot(loseSound, loseSoundvolume);
        }
    }
}
