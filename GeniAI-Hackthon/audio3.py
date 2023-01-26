import speech_recognition as sr
import librosa
import os
from tkinter import *
from sklearn.neural_network import MLPClassifier

clf=False

# Create a function to record the user's voice
def record_voice():
    r = sr.Recognizer()
    with sr.Microphone() as source:
        print("Say something!")
        audio = r.listen(source)
        user_voice = r.record(audio, format='wav')
        #user_voice = r.record(audio)
        user_voice.export("user_voice.wav", format='wav')
        print("Voice recorded and saved.")

# Create a function to extract MFCC features from the recorded voice
def extract_mfccs():
    user_voice, sr = librosa.load("user_voice.wav")
    mfccs = librosa.feature.mfcc(user_voice, sr=sr)
    return mfccs

# Create a function to train a model on the extracted MFCCs
def train_model(mfccs):
    clf = MLPClassifier()
    clf.fit(mfccs)
    return clf

# Create a function to recognize the user's voice
def recognize_voice(clf):
    r = sr.Recognizer()
    with sr.Microphone() as source:
        print("Say something!")
        audio = r.listen(source)
        new_voice = r.record(audio, format='wav')
        new_voice.export("new_voice.wav", format='wav')
        new_mfccs = librosa.feature.mfcc(librosa.load("new_voice.wav"))
        result = clf.predict(new_mfccs)
        if result == 1:
            print("This is the user's voice.")
        else:
            print("This is not the user's voice.")

# Create a GUI for the application using tkinter
root = Tk()
root.title("Voice Recognition")

record_button = Button(root, text="Record Voice", command=record_voice)
record_button.pack()

train_button = Button(root, text="Train Model", command=lambda: train_model(extract_mfccs()))
train_button.pack()

recognize_button = Button(root, text="Recognize Voice", command=lambda: recognize_voice(clf))
recognize_button.pack()

root.mainloop()

