
window.onload = function () {
    const form = document.getElementById('Accountinformationform'); // replace with your form's ID
    const inputName = document.querySelector("input[id='InputAccountName']"); // Assuming ID
    const inputType = document.querySelector("input[id='InputAccountType']"); // Assuming ID for the new input field
    const submitButton = document.getElementById("editanddeleteModal"); // Assuming ID
    const SecondButton = document.getElementById("formbtn"); // Assuming ID
    const ThirdButton = document.getElementById("SubmitUpdateAccount"); // Assuming ID for the third button
    const RegretButtons = document.querySelectorAll('.RegretButtons');

    // Hide the second button initially
    SecondButton.hidden = true;

    let enterKeyPressCount = 0; // Variable to track the number of times the Enter key has been pressed

    inputName.addEventListener("input", updateValue);
    inputType.addEventListener("input", updateValue); // Add event listener to the new input field

    function isValidLength(input, min, max) {
        let length = input.value.length;
        return length > min && length <= max;
    }

    function updateValue(e) {
        if (!isValidLength(inputName, 1, 30) || !isValidLength(inputType, 1, 30)) {  // Check input length of both target elements
            submitButton.hidden = true;
            SecondButton.hidden = false;
        } else {
            SecondButton.hidden = true;
            submitButton.hidden = false;
        }
    }

    form.addEventListener('keydown', function (event) {
        if (event.key === 'Enter') {
            event.preventDefault(); // prevent form submission
            // Check which button to simulate a click on
            if (!isValidLength(inputName, 1, 30) || !isValidLength(inputType, 1, 30)) {
                SecondButton.click();
            } else if (enterKeyPressCount === 0) {
                submitButton.click();
                enterKeyPressCount++;
            } else {
                ThirdButton.click();
            }
        }
    });

    function resetKeyPressCount() {
        enterKeyPressCount = 0;
    }

    RegretButtons.forEach(button => {
        button.addEventListener('click', resetKeyPressCount);
    });

    document.addEventListener('keydown', function (event) {
        if (event.key === 'Escape') {
            resetKeyPressCount();
        }
    });

    // Trigger the input event manually for both input fields
    var event = new Event('input', {
        bubbles: true,
        cancelable: true,
    });
    inputName.dispatchEvent(event);
    inputType.dispatchEvent(event); // Dispatch event for the new input field
}