import React, { useState } from 'react'

export default function Aud2Image() {

    const [isFileSelected,setIsFileSelected] = useState(false)
    const[url,setUrl] = useState('')
    const [isloading,setIsLoadin] = useState(false)

   
    const submitBtnFunc = () =>
    {
        const uploadInput = document.getElementById('upload');
        setUrl('')
        if (uploadInput.files.length > 0) {
            setIsFileSelected(true);
          }
    }

    const handleDownload = () => {
      const link = document.createElement('a');
      link.href = url;
      link.download = 'img.jpg';
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    };

    const handleFileUpload = async () => {
      setIsLoadin(true)
      const file = document.getElementById('upload').files[0];
  
      const formData = new FormData();
      formData.append('file', file);
  
      try {
        const response = await fetch('http://localhost:5000/upload-aud', {
          method: 'POST',
          body: formData,
        });

        if (response.ok) {
          const blob = await response.blob();
          const url = window.URL.createObjectURL(blob);
          setUrl(url);
        } else {
          console.error('File upload failed');
        }
      } catch (error) {
        console.error('Error uploading file:', error);
      }
      setIsLoadin(false)
    };
    

  return (
    <div className="float-box" style={{width:"30%",float:"left",margin:"5%",padding:"5%"}}>
        <h1 style={{textAlign:"center"}}> Audio To Image </h1>
        <label htmlFor="upload" className="upload-label">please select a wav file to upload</label> <br/>
        <input onChange= {submitBtnFunc} type="file" id="upload"/> <br/>
        <span hidden={!isloading}><img style={{height:'2rem',opacity:'50%'}} alt='loading...' src='./settings.gif'/><br/></span>
        <button onClick={handleFileUpload} className="submit-btn" disabled={!isFileSelected}>Upload</button> &nbsp;
        <button onClick={handleDownload} className="submit-btn" disabled={!url}>Download</button>
    </div>
  )
}
