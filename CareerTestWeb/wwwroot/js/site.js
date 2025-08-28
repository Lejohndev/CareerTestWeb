// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification


    document.addEventListener('DOMContentLoaded', function() {
            // Elements
            const introSection = document.getElementById('intro-section');
    const questionsSection = document.getElementById('questions-section');
    const resultsSection = document.getElementById('results-section');
    const startBtn = document.getElementById('start-btn');
    const nextBtn = document.getElementById('next-btn');
    const prevBtn = document.getElementById('prev-btn');
    const restartBtn = document.getElementById('restart-btn');

    // Event Listeners
    startBtn.addEventListener('click', function() {
        introSection.style.display = 'none';
    questionsSection.style.display = 'block';
            });

    nextBtn.addEventListener('click', function() {
        // In a real application, this would check if it's the last question
        // and show results if so
        questionsSection.style.display = 'none';
    resultsSection.style.display = 'block';
            });

    restartBtn.addEventListener('click', function() {
        resultsSection.style.display = 'none';
    introSection.style.display = 'block';
            });

            // In a real implementation, you would have:
            // 1. A list of questions to iterate through
            // 2. API integration to send answers and receive results
            // 3. More sophisticated state management

            // Example of API integration (pseudo-code):
            /*
            async function submitAnswers(answers) {
                try {
                    const response = await fetch('https://personalitypolice.com/api', {
        method: 'POST',
    headers: {
        'Content-Type': 'application/json'
                        },
    body: JSON.stringify(answers)
                    });

    if (!response.ok) {
                        throw new Error('API request failed');
                    }

    const data = await response.json();
    displayResults(data);
                } catch (error) {
        console.error('Error:', error);
    alert('Có lỗi xảy ra khi gửi đánh giá. Vui lòng thử lại sau.');
                }
            }
    */
        });
