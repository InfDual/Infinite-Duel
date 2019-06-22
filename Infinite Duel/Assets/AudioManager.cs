using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duel.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField]
        private AudioClip[] themes;

        [SerializeField]
        private AudioSource sourceOne;

        [SerializeField]
        private AudioSource sourceTwo;

        private AudioSource activeSource;

        [SerializeField]
        private float fadeDuration;

        private Coroutine crossfadeCoroutine;

        public void SwapToSong(int songIndex)
        {
            if (crossfadeCoroutine != null)
            {
                StopCoroutine(crossfadeCoroutine);
            }
            crossfadeCoroutine = StartCoroutine(Fade(songIndex));
        }

        private IEnumerator Fade(int newSongIndex)
        {
            AudioSource tempSource = sourceTwo;
            sourceTwo = sourceOne;
            sourceOne = tempSource;

            sourceTwo.clip = themes[newSongIndex];
            sourceTwo.Play();
            float timeRemaining = fadeDuration;
            while (timeRemaining > 0)
            {
                sourceOne.volume = timeRemaining / fadeDuration;
                sourceTwo.volume = (fadeDuration - timeRemaining) / fadeDuration;

                yield return null;
                timeRemaining -= Time.deltaTime;
            }

            sourceOne.volume = 0;
            sourceTwo.volume = 1;
        }
    }
}