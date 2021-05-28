//https://stackoverflow.com/questions/61763796/record-webcam-video-using-javascript

function onStart(options)  {
    let preview = document.getElementById("preview");
    let recording = document.getElementById("recording");
    let startButton = document.getElementById("startButton");
    let stopButton = document.getElementById("stopButton");
    let downloadButton = document.getElementById("downloadButton");
    let logElement = document.getElementById("log");

    let recordingTimeMS = 9000;

    function log(msg) {
        logElement.innerHTML += msg + "\n";
    }

    function wait(delayInMS) {
        return new Promise(resolve => setTimeout(resolve, delayInMS));
    }

    function startRecording(stream, lengthInMS) {
        let recorder = new MediaRecorder(stream);
        let data = [];

        recorder.ondataavailable = event => data.push(event.data);
        recorder.start();
        log(recorder.state + " for " + (lengthInMS / 1000) + " seconds...");

        let stopped = new Promise((resolve, reject) => {
            recorder.onstop = resolve;
            recorder.onerror = event => reject(event.name);
        });

        let recorded = wait(lengthInMS).then(
            () => recorder.state == "recording" && recorder.stop()
        );

        return Promise.all([
            stopped,
            recorded
        ])
            .then(() => data);
    }
    function stop(stream) {
        stream.getTracks().forEach(track => track.stop());
    }
    startButton.addEventListener("click", function () {
        navigator.mediaDevices.getUserMedia({
            video: true,
            audio: false
        }).then(stream => {
            preview.srcObject = stream;
            downloadButton.href = stream;
            preview.captureStream = preview.captureStream || preview.mozCaptureStream;
            return new Promise(resolve => preview.onplaying = resolve);
        }).then(() => startRecording(preview.captureStream(), recordingTimeMS))
            .then(recordedChunks => {
                console.log("recordedChunks");
                console.log(recordedChunks);
                let recordedBlob = new Blob(recordedChunks, { type: "video/mp4" });
                console.log("recordedBlob");

                var reader = new FileReader();
                reader.readAsDataURL(recordedBlob);
                reader.onloadend = function () {
                    var base64String = reader.result;
                    console.log('Base64 String - ', base64String);

                    // Simply Print the Base64 Encoded String,
                    // without additional data: Attributes.
                    console.log('Base64 String without Tags- ',
                        base64String.substr(base64String.indexOf(', ') + 1));
                }
                console.log(recordedBlob);
                recording.src = URL.createObjectURL(recordedBlob);
                downloadButton.href = recording.src;
                downloadButton.download = "RecordedVideo.mp4";

                log("Successfully recorded " + recordedBlob.size + " bytes of " +
                    recordedBlob.type + " media.");
            })
            .catch(log);
    }, false);

    stopButton.addEventListener("click", function () {
        stop(preview.srcObject);
    }, false);
}

window.WebCamFunctions = {
    start: (options) => { onStart(options); }
};