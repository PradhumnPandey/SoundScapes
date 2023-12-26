const express = require('express');
const multer = require('multer');
const path = require('path')
const cors = require('cors'); // Import the cors package
const app = express();
const { spawn } = require('child_process');
const fs = require('fs')

app.use(cors());
// Set up multer for file uploads
const imgstorage = multer.diskStorage({
  destination: function (req, file, cb) {
    cb(null, 'Input/'); // Specify the upload directory
  },
  filename: function (req, file, cb) {
    cb(null, 'img.jpg'); // Use the original file name
  },
});

// Set up multer for file uploads
const audstorage = multer.diskStorage({
  destination: function (req, file, cb) {
    cb(null, 'Input/'); // Specify the upload directory
  },
  filename: function (req, file, cb) {
    cb(null, 'output.wav'); // Use the original file name
  },
});

const imgupload = multer({ storage: imgstorage });
const audupload = multer({storage:audstorage});

function runChildProcess(command) {
  return new Promise((resolve, reject) => {
    const child = spawn('C:/Users/v-pandeyprad/source/repos/SoundScapes/Driver/bin/Debug/net6.0/Driver.exe', [], {
      stdio: 'pipe',
      shell: true, // For Windows
    });

    child.stdin.write(command);
    child.stdin.end();

    child.stdout.on('data', (data) => {
      console.log(`Output: ${data.toString()}`);
    });
    child.stderr.on('data', (data) => {
      console.error(`Error: ${data.toString()}`);
    });

    child.on('close', (code) => {
      if (code === 0) {
        resolve();
      } else {
        reject(new Error(`Child process exited with code ${code}`));
      }
    });

    child.on('error', (err) => {
      reject(err);
    });
  });
}

app.post('/upload-img', imgupload.single('file'), async (req, res) => {
  if (!req.file) {
    return res.status(400).send('No files were uploaded.');
  }

  const uploadedFilePath = path.join(__dirname, req.file.path);
  const destinationPath = 'destination_folder/' + req.file.originalname;

  fs.rename(uploadedFilePath, destinationPath, (err) => {
    console.log('File saved successfully');
  });

  try {
    await runChildProcess('1\n1\n');
    res.sendFile(path.join(__dirname, 'output.wav'));
  } catch (error) {
    console.error('Error occurred while processing:', error);
    res.status(500).send('Error occurred while processing the file');
  }
});





app.post('/upload-aud', audupload.single('file'), async (req, res) => {
  if (!req.file) {
    return res.status(400).send('No files were uploaded.');
  }
  const uploadedFilePath = path.join(__dirname, req.file.path);
  const destinationPath = 'destination_folder/' + req.file.originalname;

  fs.rename(uploadedFilePath, destinationPath, (err) => {
    console.log('File saved successfully');
  });

  try {
    await runChildProcess('2\n1\n');
    res.sendFile(path.join(__dirname, 'imgbw.jpeg'));
  } catch (error) {
    console.error('Error occurred while processing:', error);
    res.status(500).send('Error occurred while processing the file');
  }
});


// Serve the React app for any other routes (catch-all)
app.get('*', (req, res) => {
    res.sendFile(path.join(__dirname, 'soundscapesfe/build/index.html'));
  });
  
app.listen(5000, () => {
  console.log('Server is running on port 5000');
});
