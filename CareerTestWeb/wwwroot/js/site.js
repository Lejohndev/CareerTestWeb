document.addEventListener('DOMContentLoaded', function () {
    // Elements
    const introSection = document.getElementById('intro-section');
    const questionsSection = document.getElementById('questions-section');
    const loadingSection = document.getElementById('loading-section');
    const resultsSection = document.getElementById('results-section');
    const startBtn = document.getElementById('start-btn');
    const nextBtn = document.getElementById('next-btn');
    const prevBtn = document.getElementById('prev-btn');
    const restartBtn = document.getElementById('restart-btn');
    const progressBar = document.querySelector('.progress-bar');
    const steps = document.querySelectorAll('.step');

    // State
    let currentQuestion = 1;
    const totalQuestions = 5;
    const answers = [];

    // Sample questions (in a real app, these would come from the API)
    const questions = [
        "Bạn thích làm việc trong môi trường như thế nào?",
        "Bạn cảm thấy hứng thú với loại công việc nào nhất?",
        "Khi làm việc nhóm, bạn thường đóng vai trò gì?",
        "Bạn xử lý thế nào khi gặp vấn đề khó khăn?",
        "Bạn đánh giá cao điều gì nhất trong công việc?"
    ];

    const options = [
        [
            "Môi trường có cấu trúc rõ ràng, ổn định",
            "Môi trường năng động, sáng tạo",
            "Môi trường giúp đỡ người khác, có ý nghĩa xã hội",
            "Môi trường cạnh tranh, hướng đến kết quả"
        ],
        [
            "Công việc phân tích, xử lý dữ liệu",
            "Công việc sáng tạo, thiết kế",
            "Công việc hỗ trợ, chăm sóc người khác",
            "Công việc quản lý, lãnh đạo"
        ],
        [
            "Người lập kế hoạch và tổ chức",
            "Người đưa ra ý tưởng sáng tạo",
            "Người hòa giải và hỗ trợ",
            "Người dẫn dắt và quyết định"
        ],
        [
            "Phân tích kỹ lưỡng từng chi tiết",
            "Tìm giải pháp sáng tạo và đổi mới",
            "Tham khảo ý kiến người khác",
            "Đưa ra quyết định nhanh chóng"
        ],
        [
            "Sự ổn định và an toàn",
            "Tự do và sáng tạo",
            "Mối quan hệ và sự giúp đỡ",
            "Thành tích và thăng tiến"
        ]
    ];

    // Event Listeners
    startBtn.addEventListener('click', startAssessment);
    nextBtn.addEventListener('click', goToNextQuestion);
    prevBtn.addEventListener('click', goToPreviousQuestion);
    restartBtn.addEventListener('click', restartAssessment);

    // Functions
    function startAssessment() {
        introSection.style.display = 'none';
        questionsSection.style.display = 'block';
        showQuestion(0);
    }

    function showQuestion(index) {
        const questionText = document.getElementById('question-text');
        questionText.textContent = questions[index];

        // Update options
        const optionLabels = document.querySelectorAll('.form-check-label');
        for (let i = 0; i < 4; i++) {
            optionLabels[i].textContent = options[index][i];
        }

        // Clear selection
        document.querySelectorAll('input[name="question"]').forEach(input => {
            input.checked = false;
        });

        // Update progress
        const progress = ((index + 1) / totalQuestions) * 100;
        progressBar.style.width = `${progress}%`;

        // Update steps
        steps.forEach((step, i) => {
            step.classList.remove('active', 'completed');
            if (i === index) {
                step.classList.add('active');
            } else if (i < index) {
                step.classList.add('completed');
            }
        });

        // Update navigation buttons
        prevBtn.disabled = index === 0;

        if (index === totalQuestions - 1) {
            nextBtn.innerHTML = 'Hoàn thành <i class="fas fa-check ms-2"></i>';
        } else {
            nextBtn.innerHTML = 'Tiếp theo <i class="fas fa-arrow-right ms-2"></i>';
        }
    }

    function goToNextQuestion() {
        const selectedOption = document.querySelector('input[name="question"]:checked');

        if (!selectedOption) {
            alert('Vui lòng chọn một câu trả lời!');
            return;
        }

        answers.push(parseInt(selectedOption.value));

        if (currentQuestion < totalQuestions) {
            showQuestion(currentQuestion);
            currentQuestion++;
        } else {
            // Finished all questions
            submitAnswers();
        }
    }

    function goToPreviousQuestion() {
        if (currentQuestion > 1) {
            answers.pop();
            currentQuestion--;
            showQuestion(currentQuestion - 1);
        }
    }

    function submitAnswers() {
        questionsSection.style.display = 'none';
        loadingSection.style.display = 'block';

        // Simulate API call to Personality Police
        setTimeout(() => {
            // In a real implementation, you would make a fetch request to the API
            /*
            fetch('https://personalitypolice.com/api', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    answers: answers,
                    // other required parameters
                })
            })
            .then(response => response.json())
            .then(data => {
                displayResults(data);
            })
            .catch(error => {
                console.error('Error:', error);
                alert('Có lỗi xảy ra khi gửi đánh giá. Vui lòng thử lại sau.');
            });
            */

            // For demo purposes, we'll simulate a response
            const simulatedResponse = {
                personalityType: "INTJ",
                personalityTitle: "Nhà chiến lược",
                personalityDescription: "Bạn có tư duy chiến lược, độc lập và quyết đoán. Bạn thích làm việc với ý tưởng và lập kế hoạch dài hạn. INTJ là những người sáng tạo và có tính cách quyết đoán, luôn tìm kiếm những giải pháp hợp lý cho các vấn đề.",
                topCareer: "Kỹ sư phần mềm",
                topCareerDescription: "Phù hợp với tính cách phân tích và kỹ thuật của bạn. Bạn có khả năng giải quyết vấn đề và làm việc với hệ thống phức tạp. Công việc này cho phép bạn sử dụng khả năng phân tích và tư duy logic để tạo ra giải pháp hiệu quả.",
                otherCareers: [
                    { name: "Phân tích dữ liệu", match: 92 },
                    { name: "Quản lý dự án", match: 88 },
                    { name: "Kiến trúc sư", match: 87 },
                    { name: "Nhà khoa học", match: 85 },
                    { name: "Luật sư", match: 82 },
                    { name: "Nhà nghiên cứu", match: 80 }
                ]
            };

            displayResults(simulatedResponse);
        }, 2000);
    }

    function displayResults(data) {
        loadingSection.style.display = 'none';
        resultsSection.style.display = 'block';

        // Update results with API data
        document.getElementById('personality-type').textContent = data.personalityType;
        document.getElementById('personality-title').textContent = data.personalityTitle;
        document.getElementById('personality-desc').textContent = data.personalityDescription;
        document.getElementById('top-career').textContent = data.topCareer;
        document.getElementById('top-career-desc').textContent = data.topCareerDescription;

        // Update other careers
        const otherCareersContainer = document.getElementById('other-careers');
        otherCareersContainer.innerHTML = '';

        data.otherCareers.forEach(career => {
            const careerElement = document.createElement('div');
            careerElement.className = 'col-md-4 mb-3';
            careerElement.innerHTML = `
                        <div class="card">
                            <div class="card-body">
                                <h6>${career.name}</h6>
                                <small class="text-muted">Phù hợp: ${career.match}%</small>
                            </div>
                        </div>
                    `;
            otherCareersContainer.appendChild(careerElement);
        });
    }

    function restartAssessment() {
        currentQuestion = 1;
        answers.length = 0;

        resultsSection.style.display = 'none';
        introSection.style.display = 'block';

        // Reset progress
        progressBar.style.width = '20%';
        steps.forEach((step, i) => {
            step.classList.remove('active', 'completed');
            if (i === 0) {
                step.classList.add('active');
            }
        });
    }
});