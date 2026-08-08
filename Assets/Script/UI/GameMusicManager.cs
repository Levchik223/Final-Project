using System;
using UnityEngine;

public class GameMusicManager : MonoBehaviour
{
  [SerializeField] AudioSource audioSource;
  [SerializeField] private AudioClip[] musicTracks;
  
  private int _currentTrackIndex;
  private bool _musicIsStopped;

  private void Start()
  {
    PlayRandomTrack();
  }

  private void Update()
  {
    if (!audioSource.isPlaying && !_musicIsStopped)
    {
      PlayRandomTrack();
    }
  }

  private void PlayRandomTrack()
  {
    if (musicTracks.Length == 0)
      return;
    int randomIndex = UnityEngine.Random.Range(0, musicTracks.Length);
    audioSource.clip = musicTracks[randomIndex];
    audioSource.Play();
  }

  public void StopMusic()
  {
    _musicIsStopped = true;
    audioSource.Stop();
  }

  private void PlayNextTrack()
  {
    if (musicTracks.Length == 0)
      return;
    audioSource.clip = musicTracks[_currentTrackIndex];
    audioSource.Play();

    _currentTrackIndex++;
    if (_currentTrackIndex >= musicTracks.Length)
    {
      _currentTrackIndex = 0;
    }
  }
}
