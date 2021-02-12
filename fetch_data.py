import pandas as pd
import numpy as np

"""

for creating Label.csv 

data = pd.read_csv('Validation.csv')
df = pd.DataFrame(data[['label_id','label']])
label_id = df['label_id'].unique()
labels = df['label'].unique()

#Creating label.csv 

label = pd.DataFrame(list(zip(labels, label_id)),
                     columns = ['label','label_id'])

label = label.sort_values('label_id')
label.to_csv('Label.csv')

"""

class DataLoader():
    def __init__(self, path_vid,path_labels,path_train = None,path_val = None,path_test =None):
        self.path_vid = path_vid
        self.path_labels = path_labels
        self.path_train = path_train
        self.path_val = path_val
        self.path_test = path_test
        
        self.get_labels(self.path_labels)
        
        if self.path_train:
            self.train_df = self.load_video_label(self.path_train)
        if self.path_val:
            self.val_df = self.load_video_label(self.path_val)
        if self.path_test:
            self.test_df = self.load_video_label(self.path_test, mode = 'input')
        
        
    def get_labels(self,path_labels):
        self.labels_df = pd.read_csv(path_labels,names = ['label','label_id'])
        self.labels = [str(label[0]) for label in self.labels_df.values]
        self.n_labels = len(self.labels)
        # have skipped one step ,implement if neccessary
        
        
    def load_video_label(self, path_sub , mode = 'label'):
        if mode == 'input':
            names = ['video_id']
        elif mode == 'label':
            names = ['video_id','label','label_id']
            
        df = pd.read_csv(path_sub,dtype=str,sep = ',',names = names)
        
        
        if mode == 'label':
            df = df[df.label.isin(self.labels)]
            lst = []
            for label in df.label_id:
                zeroes = np.zeros(27,dtype = int)
                zeroes[int(label)] = 1
                zeroes = tuple(zeroes)
                lst.append(zeroes)
            df['encoded_label'] = [i for i in lst]
            
        return df
            
            