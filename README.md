# KTANEHue
A simple and easy to use Philips Hue extension for Keep Talking And Nobody Explodes.

## Installation
1. Begin by either cloning this repository or downloading the zip file directly from the online GitHub repository.


2. Find the KTANEHue.exe file in the base directory of the repository (or build it if you open it with VS) and move it to the Keep Talking And Nobody Explodes game directory. This is typically "C:/Program Files (x86)/Steam/steamapps/common/Keep Talking And Nobody Explodes" but depending on where your steam library is installed, the location might be different.


3. Next is to find your Philips Hue IP. The easiest way to do this is by using [Philip's broker discovery process](https://www.meethue.com/api/nupnp). You will want to take note of the "internalipaddress" of the bridge you want to use.


4. Once we know our Hue IP, we can ask the bridge to generate a user key for us. To do so, go to your Hue API address http://<bridge ip address>/debug/clip.html.
  Put the following values into their appropriate fields, replacing values surrounded by <>:
  * URL: `http://<bridge ip address>/api`
  * Body: `{"devicetype":"KTANEHue#<your name>"}`
  * Method: `POST`

5. **Please Ensure that you press the link button on your Hue device before sending the POST request or it will not work.**
If the command ran successfully, you'll have a JSON response that contains a "username" field. Copy the username and save it for the final step.

6. We now need to determine which lights we want to be affected by the program. To do so, go to your Hue API address http://<bridge ip address>/debug/clip.html.
  Put the following values into their appropriate fields, replacing values surrounded by <>:
  * URL: `http://<bridge ip address>/api/<username from step 4>/lights`
  * Method: `GET`
  Once you've run this command, you'll get a list of all the lights connected to your bridge. Make a list of all the top level JSON ID numbers for the lights you want to use.
  
7. Open a cmd window in the directory where you installed the KTANEHue.exe program. Run the following command:
  `KTANEHue.exe <Hue Bridge IP> <username> <light 1> <light 2> <light 3> <light ...>`
  Where you add as many lights as you want the program to use
  
  

