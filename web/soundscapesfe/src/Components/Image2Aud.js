import React, { useState } from 'react'

export default function Image2Aud() {

    const [isFileSelected,setIsFileSelected] = useState(false)
    const[url,setUrl] = useState('')
    const [isloading,setIsLoadin] = useState(false)
    const submitBtnFunc = () =>
    {
        const uploadInput = document.getElementById('uploadImg');
        setUrl('')
    
        if (uploadInput.files.length > 0) {
            setIsFileSelected(true);
          }
    }

    const handleDownload = () => {
      const link = document.createElement('a');
      link.href = url;
      link.download = 'output.wav';
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    };
    
    const handleFileUpload = async () => {
       setIsLoadin(true)
        const file = document.getElementById('uploadImg').files[0];
    
        const formData = new FormData();
        formData.append('file', file);
    
        try {
          const response = await fetch('http://localhost:5000/upload-img', {
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
    <div className="float-box" style={{width:"29%",float:"right",margin:"5%",padding:"5%"}}>
        <h1 style={{textAlign:"center"}}> Image To Audio </h1>
        <label htmlFor="upload" className="upload-label">please select a jpg file to upload</label> <br/>
        <input accept=".wav,.jpg,.png" onChange={submitBtnFunc} type="file" id="uploadImg"/> <br/>
        <span hidden={!isloading}><img style={{height:'2rem',opacity:'50%'}} alt='loading...' src='./settings.gif'/><br/></span>
        <button onClick={handleFileUpload} className="submit-btn" disabled={!isFileSelected}>Upload</button> &nbsp;
        <button onClick={handleDownload} className="submit-btn" disabled={!url}>Download</button>
    </div>
  )
}
