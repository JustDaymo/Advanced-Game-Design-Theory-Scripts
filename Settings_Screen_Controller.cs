using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Linq;

public class Settings_Screen_Controller : MonoBehaviour
{

    public AudioMixer VolumeControl;
    public Dropdown ResolutionDropdown;

    Resolution[] Resolutions;   // An array of options the player may select for resolutions.

    void Start()
    {
        Resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
        // Gets the list of resolutions available and removes all the duplicates that are just different refresh rates.

        ResolutionDropdown.ClearOptions();  // Clears the list of the default options.

        List<string> Options = new List<string>();  // Creates a list that will be the options.

        int CurrentResolution = 0;

        for (int i = 0; i < Resolutions.Length; i++)
        {
            string Option = Resolutions[i].width + " x " + Resolutions[i].height;   //  Creates a string thing that displays the resolutions currently being looked at.
            Options.Add(Option);    // Adds the options to the list.

            if (Resolutions[i].width == Screen.width &
                Resolutions[i].height == Screen.height)   // If the current resolution is equal to the resolution being looked at.
            {
                CurrentResolution = i; // Sets CurrentResolution to what is being looked at.
            }
        }

        ResolutionDropdown.AddOptions(Options);
        ResolutionDropdown.value = CurrentResolution;   // Sets the dropdown to the current resolution.
        ResolutionDropdown.RefreshShownValue(); 
    }

    public void SetResolution(int CurrentResolution)
    {
        Resolution Resolution = Resolutions[CurrentResolution]; // Sets up Resolution to be recongized as a resolution and sets it to match the current one.
        Screen.SetResolution(Resolution.width, Resolution.height, Screen.fullScreen); // Sets the game's resolution with the numbers in CurrentResolution.
    }

    public void SetVolume(float Volume)
    {
        VolumeControl.SetFloat("Volume", Volume); // Sets the float of the volume to match the one in game which goes from -80 to 0.
    }

    public void SetFullscreen (bool FullscreenTicked)
    {
        Screen.fullScreen = FullscreenTicked;   // If the fullscreen box is ticked, set the screen to full.
    }    

}
