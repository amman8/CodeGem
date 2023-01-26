import sys
from PyQt5.QtWidgets import QApplication, QLabel, QPushButton, QVBoxLayout, QWidget
import speech_recognition as sr

class VoiceRecognitionApp(QWidget):
    def __init__(self):
        super().__init__()
        self.title = 'Voice Recognition App'
        self.left = 10
        self.top = 10
        self.width = 320
        self.height = 200
        self.initUI()

    def initUI(self):
        self.setWindowTitle(self.title)
        self.setGeometry(self.left, self.top, self.width, self.height)

        # Initialize recognizer class (for recognizing the speech)
        self.r = sr.Recognizer()

        # Create a label to display instructions
        self.label = QLabel("Talk for 10 seconds, to train the AI on your voice", self)
        self.label.move(10, 10)
        self.label.resize(280, 30)
        # Create a button to start training
        self.train_button = QPushButton("Train", self)
        self.train_button.move(10, 50)
        self.train_button.clicked.connect(self.train)

        # Create a button to start recognition
        self.recognize_button = QPushButton("Recognize", self)
        self.recognize_button.move(210, 50)
        self.recognize_button.clicked.connect(self.recognize)

        # Create a label to display recognition result
        self.result_label = QLabel("", self)
        self.result_label.move(10, 100)
        self.result_label.resize(280, 30)

        self.show()

    def train(self):
        # Reading Microphone as source
        # listening the speech and store in audio_text variable
        with sr.Microphone() as source:
            self.label.setText("Talk for 10 seconds, to train the AI on your voice")
            audio_text = self.r.listen(source, timeout=10)

        # Extracting voice sample from audio_text
        self.sample_voice = self.r.recognize_google(audio_text, show_all=True)
        self.label.setText("Training complete")

    def recognize(self):
        # Reading Microphone as source
        # listening the speech and store in audio_text variable
        with sr.Microphone() as source:
            self.label.setText("Talk")
            print("Talk")
            audio_text = self.r.listen(source)
            self.label.setText("Time over, thanks")
            print("Time Over Thanks")

        # Extracting voice from audio_text
        voice = self.r.recognize_google(audio_text, show_all=True)

        try:
            # Compare the voice sample with the new voice
            if self.sample_voice['alternative'][0]['transcript'] == voice['alternative'][0]['transcript']:
                self.result_label.setText("Welcome Master!")
            else:
                self.result_label.setText("Sorry, I did not recognize your voice.")
       
        except:
            self.result_label.setText("Sorry, I did not get that")

if __name__ == '__main__':
    app = QApplication(sys.argv)
    ex = VoiceRecognitionApp()
    sys.exit(app.exec_())
