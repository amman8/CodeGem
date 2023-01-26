import speech_recognition as sr
import tkinter as tk

class VoiceRecognitionApp:
    def __init__(self, master):
        self.master = master
        master.title("Voice Recognition App")

        # Initialize recognizer class (for recognizing the speech)
        self.r = sr.Recognizer()

        # Create a label to display instructions
        self.label = tk.Label(master, text="Talk for 10 seconds, to train the AI on your voice")
        self.label.pack()

        # Create a button to start training
        self.train_button = tk.Button(master, text="Train", command=self.train)
        self.train_button.pack()

        # Create a button to start recognition
        self.recognize_button = tk.Button(master, text="Recognize", command=self.recognize)
        self.recognize_button.pack()

        # Create a label to display recognition result
        self.result_label = tk.Label(master, text="")
        self.result_label.pack()

    def train(self):
        # Reading Microphone as source
        # listening the speech and store in audio_text variable
        with sr.Microphone() as source:
            self.label.config(text="Talk for 10 seconds, to train the AI on your voice")
            audio_text = self.r.listen(source, timeout=10)

        # Extracting voice sample from audio_text
        self.sample_voice = self.r.recognize_google(audio_text, show_all=True)
        self.label.config(text="Training complete")

    def recognize(self):
        # Reading Microphone as source
        # listening the speech and store in audio_text variable
        with sr.Microphone() as source:
            self.label.config(text="Talk")
            audio_text = self.r.listen(source)
            self.label.config(text="Time over, thanks")

        # Extracting voice from audio_text
        voice = self.r.recognize_google(audio_text, show_all=True)

        try:
            # Compare the voice sample with the new voice
            if self.sample_voice['alternative'][0]['transcript'] == voice['alternative'][0]['transcript']:
                self.result_label.config(text="Welcome Master!")
            else:
                self.result_label.config(text="Sorry, I did not recognize your voice.")
        except:
            self.result_label.config(text="Sorry, I did not get that")

root = tk.Tk()
app = VoiceRecognitionApp(root)
root.mainloop()
