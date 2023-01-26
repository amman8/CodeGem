#from pydub import AudioSegment

import speech_recognition as sr

# Initialize recognizer class (for recognizing the speech)
r = sr.Recognizer()

# Reading Microphone as source
# listening the speech and store in audio_text variable
with sr.Microphone() as source:
    print("Talk for 10 seconds, to train the AI on your voice")
    audio_text = r.listen(source, timeout=5)

# Extracting voice sample from audio_text
sample_voice = r.recognize_google(audio_text, show_all=True)

while True:
    # Reading Microphone as source
    # listening the speech and store in audio_text variable
    with sr.Microphone() as source:
        print("Talk")
        audio_text = r.listen(source)
        print("Time over, thanks")

    # Extracting voice from audio_text
    voice = r.recognize_google(audio_text, show_all=True)

    try:
        # Compare the voice sample with the new voice
        if sample_voice['alternative'][0]['transcript'] == voice['alternative'][0]['transcript']:
            print("Welcome Master!")
        else:
            print("Sorry, I did not recognize your voice.")
    except:
        print("Sorry, I did not get that")
