import wave
import matplotlib.pyplot as plt
import numpy as np

obj = wave.open("audio.wav","rb")

s_frame=obj.getframerate()
n_sample = obj.getnframes()
signal_wav = obj.readframes(-1)

obj.close()

t_audio = n_sample/s_frame

print(t_audio)


signal_arr=np.frombuffer(signal_wav,dtype=np.init16)

time = np.linspace(0,t_audio,num=n_sample)


plt.figure(figsize=(15,5))
plt.plot(time,signal_arr)
plt.title("Audio Signal")
plt.ylabel("Signal wave")
plt.xlabel("Time(s")
plt.xlim(0,t_audio)
plt.show()