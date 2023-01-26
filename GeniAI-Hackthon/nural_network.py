import numpy as np
import sounddevice as sd
import keras
from keras.models import Sequential
from keras.layers import Dense, Dropout, Activation

# Recording the audio for 10 seconds
fs = 44100  # Sample rate
seconds = 10  # Duration of recording

recording = sd.rec(int(fs * seconds), samplerate=fs, channels=2)
sd.wait()  # Wait until recording is finished

# Save the recording
np.save("voice_sample.npy", recording)

# Load the saved recording
voice_sample = np.load("voice_sample.npy")

# Reshape the recording for the Neural Network
X = voice_sample.reshape(-1, 2 * fs * seconds)

# Define the Neural Network
model = Sequential()
model.add(Dense(32, input_dim=2 * fs * seconds, activation='relu'))
model.add(Dense(16, activation='relu'))
model.add(Dense(1, activation='sigmoid'))

# Compile the Neural Network
model.compile(loss='binary_crossentropy', optimizer='adam', metrics=['accuracy'])

# Train the Neural Network
model.fit(X, [1], epochs=20, batch_size=32)

# Ask the user to speak again
recording = sd.rec(int(fs * seconds), samplerate=fs, channels=2)
sd.wait()  # Wait until recording is finished

# Reshape the new recording
X_new = recording.reshape(-1, 2 * fs * seconds)

# Use the trained Neural Network to recognize the voice
prediction = model.predict(X_new)

if prediction > 0.5:
    print("Voice matched!")
else:
    print("Sorry, I did not recognize your voice.")
