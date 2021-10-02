from fetch_data import DataLoader
from utils import mkdirs
import os
import image as img
from keras_preprocessing.image import ImageDataGenerator

import keras
from keras import layers
from keras import optimizers
from keras.models import Sequential
from keras.layers import Dense, Flatten, Conv3D, MaxPooling3D, Dropout, BatchNormalization
from keras.utils import to_categorical
import numpy as np
import matplotlib.pyplot as plt

from keras.optimizers import SGD

# path....
data_root = r"C:\Users\Bhanu Pandey\Desktop\Subset"

csv_label = r"C:\Users\Bhanu Pandey\Desktop\Subset\Label.csv"
csv_train = r"C:\Users\Bhanu Pandey\Desktop\Subset\Train.csv"
csv_test = r"C:\Users\Bhanu Pandey\Desktop\Subset\Test.csv"
csv_val = r"C:\Users\Bhanu Pandey\Desktop\Subset\Validation.csv"
data_vid = r"C:\Users\Bhanu Pandey\Desktop\Subset"
model_name = "3D_cnn_model"
model_path = r"C:\Users\Bhanu Pandey\Desktop\Subset\Model"

path_model = os.path.join(data_root,model_path,model_name)
path_vid  = os.path.join(data_root,data_vid)
path_test = os.path.join(data_root,csv_test)
path_train = os.path.join(data_root,csv_train)
path_val = os.path.join(data_root,csv_val)
path_label = os.path.join(data_root,csv_label)

target_size = (64,64)
nb_frames = 16
skip = 1
batch_size = 16

data = DataLoader(data_vid,path_label,path_train,path_val)

mk(path_model,0o755)
mkdirs(os.path.join(path_model,"graph"),0o755)
dirs

def get_model(width=64, height=64 ,depth=16):
    """Build a 3D convolutional neural network model."""

    inputs = keras.Input((depth,width, height, 3))
   
    x = layers.Conv3D(filters=64, kernel_size=3, activation="relu",padding = 'same')(inputs)
    x = layers.MaxPool3D(pool_size=2)(x)
    x = layers.BatchNormalization()(x)

    x = layers.Conv3D(filters=64, kernel_size=3, activation="relu",padding = 'same')(x)
    x = layers.MaxPool3D(pool_size=2)(x)
    x = layers.BatchNormalization()(x)

    x = layers.Conv3D(filters=128, kernel_size=3, activation="relu",padding = 'same')(x)
    x = layers.MaxPool3D(pool_size=2)(x)
    x = layers.BatchNormalization()(x)

    x = layers.Conv3D(filters=256, kernel_size=3, activation="relu",padding = 'same')(x)
    x = layers.MaxPool3D(pool_size=2)(x)
    x = layers.BatchNormalization()(x)

    x = layers.GlobalAveragePooling3D()(x)
    x = layers.Dense(units=512, activation="relu")(x)
    x = layers.Dropout(0.3)(x)

    outputs = layers.Dense(units=27, activation="softmax")(x)

    # Define the model.
    model = keras.Model(inputs, outputs, name="3dcnn")
    return model



gen = img.ImageDataGenerator()
gen_train = gen.flow_video_from_dataframe(data.train_df,
                                          os.path.join(path_vid,"Train"),
                                          path_classes = path_label,
                                          x_col='video_id',
                                          y_col='label_id',
                                          target_size = target_size,
                                          nb_frames = nb_frames,
                                          batch_size = batch_size,
                                          skip = skip,
                                          has_ext = True)

gen_val = gen.flow_video_from_dataframe(data.val_df,
                                          os.path.join(path_vid,"Validation"),
                                          path_classes = path_label,
                                          x_col='video_id',
                                          y_col='label_id',
                                          target_size = target_size,
                                          nb_frames = nb_frames,
                                          batch_size = batch_size,
                                          skip = skip,
                                          has_ext = True)


# Build model.
model = get_model(width=64, height=64, depth=16)
model.summary()



# Compile model.


epochs = 100
learning_rate = 0.1
decay_rate = learning_rate / epochs
momentum = 0.8


model.compile(optimizer=SGD(lr=learning_rate, momentum=momentum, decay=decay_rate, nesterov=False),
              loss='categorical_crossentropy',
              metrics=['accuracy'])



# Define callbacks.
checkpoint_cb = keras.callbacks.ModelCheckpoint(
    "3d_image_classification.h5", save_best_only=True
)
early_stopping_cb = keras.callbacks.EarlyStopping(monitor="val_acc", patience=15)

# Train the model, doing validation at the end of each epoch


model.fit_generator(
    generator = gen_train,
    validation_data=gen_val,
    epochs=epochs,
    shuffle=True,
    verbose=1,
    workers = 1,
    callbacks=[checkpoint_cb, early_stopping_cb]
)



"""
model.fit(
        train_generator,
        steps_per_epoch=2000,
        epochs=50,
        validation_data=validation_generator,
        validation_steps=800)

"""