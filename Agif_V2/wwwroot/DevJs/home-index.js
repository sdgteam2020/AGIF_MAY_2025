document.addEventListener('DOMContentLoaded', function () {
    // Initialize slider functionality
    initializeImageSlider();
});

function initializeImageSlider() {
    const slides = document.querySelector('.slides');
    const dots = document.querySelectorAll('.dot');

    // Check if slider elements exist
    if (!slides || !dots.length) {
        console.warn('Slider elements not found');
        return;
    }

    let index = 0;
    const totalSlides = dots.length;
    const slideInterval = 3000; // 3 seconds

    
    function showSlide(i) {
        index = (i + totalSlides) % totalSlides; // Ensure circular navigation
        slides.style.transform = `translateX(${-index * 100}%)`;

        // Update active dot
        updateActiveDot(index);
    }

    function updateActiveDot(activeIndex) {
        dots.forEach((dot, i) => {
            if (i === activeIndex) {
                dot.classList.add('active');
            } else {
                dot.classList.remove('active');
            }
        });
    }
    
    function startAutoPlay() {
        return setInterval(() => {
            showSlide(index + 1);
        }, slideInterval);
    }
    
    function stopAutoPlay(intervalId) {
        clearInterval(intervalId);
    }

    // Initialize first slide
    showSlide(0);

    // Start auto-play
    let autoPlayInterval = startAutoPlay();

    // Add click event listeners to dots
    dots.forEach((dot, i) => {
        dot.addEventListener('click', function () {
            // Stop current auto-play
            stopAutoPlay(autoPlayInterval);

            // Show clicked slide
            showSlide(i);

            // Restart auto-play
            autoPlayInterval = startAutoPlay();
        });
    });

    // Optional: Pause auto-play on hover
    const sliderContainer = document.querySelector('.slider');
    if (sliderContainer) {
        sliderContainer.addEventListener('mouseenter', function () {
            stopAutoPlay(autoPlayInterval);
        });

        sliderContainer.addEventListener('mouseleave', function () {
            autoPlayInterval = startAutoPlay();
        });
    }
}

const modal = document.getElementById("modal");
let isModalOpen = false;
let isSending = false;
let welcomeShown = false; // ✅ Track if welcome was already shown

function toggleModal() {
    const button = document.getElementById("chat-toggle-btn");

    if (isModalOpen) {
        modal.style.right = "-40vw";
        modal.style.opacity = "0";

        button.innerHTML = `   <svg xmlns="http://www.w3.org/2000/svg" height="26" width="32"
                                        viewBox="0 0 640 512">
                                        <path fill="#ffffff"
                                            d="M320 0c17.7 0 32 14.3 32 32l0 64 120 0c39.8 0 72 32.2 72 72l0 272c0 39.8-32.2 72-72 72l-304 0c-39.8 0-72-32.2-72-72l0-272c0-39.8 32.2-72 72-72l120 0 0-64c0-17.7 14.3-32 32-32zM208 384c-8.8 0-16 7.2-16 16s7.2 16 16 16l32 0c8.8 0 16-7.2 16-16s-7.2-16-16-16l-32 0zm96 0c-8.8 0-16 7.2-16 16s7.2 16 16 16l32 0c8.8 0 16-7.2 16-16s-7.2-16-16-16l-32 0zm96 0c-8.8 0-16 7.2-16 16s7.2 16 16 16l32 0c8.8 0 16-7.2 16-16s-7.2-16-16-16l-32 0zM264 256a40 40 0 1 0 -80 0 40 40 0 1 0 80 0zm152 40a40 40 0 1 0 0-80 40 40 0 1 0 0 80zM48 224l16 0 0 192-16 0c-26.5 0-48-21.5-48-48l0-96c0-26.5 21.5-48 48-48zm544 0c26.5 0 48 21.5 48 48l0 96c0 26.5-21.5 48-48 48l-16 0 0-192 16 0z" />
                                    </svg>`
    } else {
        modal.style.right = "97px";
        modal.style.opacity = "1";
        button.innerHTML = `<i class="fas fa-times text-white-600 text-2xl"></i>`;
        if (!welcomeShown) {
            showWelcomeMessage();
            welcomeShown = true;
        }
    }
    // showWelcomeMessage();
    sendQuery("hi", true);     // Silent repeat call
      
    setInterval(() => {
        console.log("⏰ Sending heartbeat 'hi' to API...");
        sendQuery("hi", true);     // Silent repeat call
    }, 240000);
    isModalOpen = !isModalOpen;
}
function showWelcomeMessage() {
    const chatbox = document.getElementById("chatbox");
    const wrapper = document.createElement("div");
    wrapper.className = "flex justify-start items-start gap-2";

    const icon = document.createElement("div");
    icon.innerHTML = '<i class="fas fa-robot text-green-600 text-lg"></i>';

    const msg = document.createElement("div");
    msg.className = "p-4 max-w-xs rounded-lg text-sm shadow bg-green-100 text-left";

    const contentWrapper = document.createElement("div");
    contentWrapper.className = "flex flex-col gap-1 items-start";

    const textDiv = document.createElement("div");
    textDiv.className = "message-text";

    // Set the welcome message
    textDiv.innerHTML = "👋 Welcome! How can I assist you today?";
    contentWrapper.appendChild(textDiv);

    msg.appendChild(contentWrapper);

    wrapper.appendChild(icon);
    wrapper.appendChild(msg);

    // Append the message to the chatbox
    chatbox.appendChild(wrapper);
    chatbox.scrollTop = chatbox.scrollHeight; // Scroll to the bottom
}

function streamText(container, text, speed = 25, done, onChunk) {
    let i = 0;
    const tick = () => {
        if (i < text.length) {
            const ch = text[i] === "\n" ? "<br>" : text[i];
            container.innerHTML += ch;
            if (onChunk) onChunk(); // ⬅️ keep scrolling as we type
            i++;
            setTimeout(tick, speed);
        } else {
            if (done) done();
        }
    };
    tick();
}


function scrollToBottom(el, smooth = true) {
    // Use smooth only if user is already near bottom
    const nearBottom = el.scrollHeight - el.scrollTop - el.clientHeight < 80;
    el.scrollTo({ top: el.scrollHeight, behavior: smooth && nearBottom ? 'smooth' : 'auto' });
}

function addMessage(role, text, stream = false, link = "", meta = {}, callback = null) {
    const chatbox = document.getElementById("chatbox");

    const wrapper = document.createElement("div");
    wrapper.className = role === "user" ? "flex justify-end items-start gap-2" : "flex justify-start items-start gap-2";

    const icon = document.createElement("div");
    icon.innerHTML = role === "user"
        ? '<i class="fas fa-user text-blue-600 text-lg"></i>'
        : '<i class="fas fa-robot text-green-600 text-lg"></i>';

    const msg = document.createElement("div");
    msg.className = "p-4 max-w-xs rounded-lg text-sm shadow " +
        (role === "user" ? "bg-blue-100 text-left" : "bg-green-100 text-left");

    const contentWrapper = document.createElement("div");
    contentWrapper.className = "flex flex-col gap-1 items-start";

    const textDiv = document.createElement("div");
    textDiv.className = "message-text";
    textDiv.innerHTML = stream ? "" : text; // if streaming, start empty

    contentWrapper.appendChild(textDiv);

    if (link) {
        const linkDiv = document.createElement("div");
        linkDiv.className = "link-container";
        linkDiv.innerHTML = link;
        contentWrapper.appendChild(linkDiv);
    }

    if (meta?.duration || meta?.scores) {
        const metaDiv = document.createElement("div");
        metaDiv.className = "text-xs text-gray-600 italic mt-1";
        let metaText = "";
        if (meta.duration) metaText += `⏱️ Response time: ${meta.duration.toFixed(2)}s<br>`;
        if (meta.scores) {
            if (typeof meta.scores === "object") {
                const scoreVals = Object.values(meta.scores).map(v => parseFloat(v).toFixed(2));
                metaText += `🔍 Similarity scores: ${scoreVals.join(", ")}`;
            } else {
                metaText += `🔍 Similarity scores: ${parseFloat(meta.scores).toFixed(2)}`;
            }
        }
        metaDiv.innerHTML = metaText;
        contentWrapper.appendChild(metaDiv);
    }

    const controls = document.createElement("div");
    controls.className = "flex gap-3 items-center text-gray-500 mt-1";

    const copyBtn = document.createElement("span");
    copyBtn.className = "cursor-pointer text-sm hover:text-blue-600";
    copyBtn.innerHTML = '<i class="fas fa-copy"></i>';
    copyBtn.title = "Copy";
    copyBtn.onclick = () => {
        navigator.clipboard.writeText(text);
        copyBtn.innerHTML = '<i class="fas fa-check text-green-600"></i>';
        setTimeout(() => (copyBtn.innerHTML = '<i class="fas fa-copy"></i>'), 1500);
    };
    controls.appendChild(copyBtn);

    if (role === "user") {
        const editBtn = document.createElement("span");
        editBtn.className = "cursor-pointer text-sm hover:text-blue-600";
        editBtn.innerHTML = '<i class="fas fa-edit"></i>';
        editBtn.title = "Edit";
        editBtn.onclick = () => makeEditable(textDiv, wrapper, text);
        controls.appendChild(editBtn);
    }

    contentWrapper.appendChild(controls);
    msg.appendChild(contentWrapper);

    // Order: [user => msg, icon], [bot => icon, msg]
    if (role === "user") {
        wrapper.appendChild(msg);
        wrapper.appendChild(icon);
    } else {
        wrapper.appendChild(icon);
        wrapper.appendChild(msg);
    }

    // ✅ Append ONCE, then scroll
    chatbox.appendChild(wrapper);
    requestAnimationFrame(() => scrollToBottom(chatbox));

    // Streaming
    if (role === "bot" && stream) {
        streamText(textDiv, text, 25, () => {
            appendRatingIcons(msg);
            controls.style.display = "flex";
            scrollToBottom(chatbox); // final scroll after stream finishes
            if (callback) callback();
        }, () => {
            // onChunk callback: keep scrolling while streaming
            scrollToBottom(chatbox);
        });
    } else {
        if (role === "bot") appendRatingIcons(msg);
        if (callback) callback();
    }
}

function sendQuery(optionalQuery = null, silent = false) {
    if (isSending) {
        console.log("Query already being processed. Please wait...");
        return;
    }

    const userQueryTime = new Date(); // ✅ Time when query is sent
    isSending = true;

    const input = document.getElementById("query");
    const sendButton = document.querySelector("button[onclick='sendQuery()']");
    const query = optionalQuery || input.value.trim();
    if (!query) {
        isSending = false;
        return;
    }

    lastUserQuery = query;

    if (!silent) {
        input.disabled = true;
        sendButton.disabled = true;
        sendButton.classList.add("opacity-50", "cursor-not-allowed");
        if (!optionalQuery) input.value = "";
        addMessage("user", query);
        document.getElementById("loading").classList.remove("hidden");
    }

    fetch("http://127.0.0.1:8000/api/asdcask/", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            query: query,
            collection_name: "chatbot_asdc"
        })
    })
        .then(res => res.json())
        .then(data => {
            const botResponseTime = new Date();
            const roundTripTime = (botResponseTime - userQueryTime) / 1000;
            console.log(`⏱️ Query sent at: ${userQueryTime.toISOString()}`);
            console.log(`✅ Response received at: ${botResponseTime.toISOString()}`);
            console.log(`⏳ Total round-trip time: ${roundTripTime.toFixed(2)} seconds`);

            if (!silent) {
                document.getElementById("loading").classList.add("hidden");
                addMessage("bot", data.answer || "Please ask a question related to the website.", true, data.link, {
                    duration: data.duration,
                    scores: data.scores
                }, () => {
                    isSending = false;
                    input.disabled = false;
                    sendButton.disabled = false;
                    sendButton.classList.remove("opacity-50", "cursor-not-allowed");
                    input.focus();
                });
            } else {
                isSending = false;
            }
        })
        .catch(() => {
            if (!silent) {
                document.getElementById("loading").classList.add("hidden");
                addMessage("bot", "❌ Error processing your request.");
                input.disabled = false;
                sendButton.disabled = false;
                sendButton.classList.remove("opacity-50", "cursor-not-allowed");
            }
            isSending = false;
        });
}

function makeEditable(textDiv, wrapper, originalText) {
    const input = document.createElement("input");
    input.type = "text";
    input.value = originalText;
    input.className = "flex-1 px-3 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-500";

    const buttonContainer = document.createElement("div");
    buttonContainer.className = "flex gap-2 mt-2";

    const cancelButton = document.createElement("button");
    cancelButton.textContent = "Cancel";
    cancelButton.className = "px-3 py-1 bg-gray-200 rounded hover:bg-gray-300";
    cancelButton.onclick = () => {
        textDiv.innerHTML = originalText;
        textDiv.contentEditable = false;
        buttonContainer.remove();
    };

    const sendButton = document.createElement("button");
    sendButton.textContent = "Send";
    sendButton.className = "px-3 py-1 bg-blue-600 text-white rounded hover:bg-blue-700";
    sendButton.onclick = () => {
        const newQuery = input.value.trim();
        if (newQuery) {
            textDiv.innerHTML = newQuery;
            textDiv.contentEditable = false;
            buttonContainer.remove();
            sendQuery(newQuery); // Re-send edited query
        }
    };

    buttonContainer.appendChild(cancelButton);
    buttonContainer.appendChild(sendButton);

    // Clear the old text and insert input + buttons
    textDiv.innerHTML = '';
    textDiv.appendChild(input);
    textDiv.appendChild(buttonContainer);
}

// Event delegation for like/dislike to handle clicks correctly
function appendRatingIcons(container) {
    const feedbackDiv = document.createElement("div");
    feedbackDiv.className = "flex justify-end gap-3 text-gray-500 text-sm mt-1";

    const likeIcon = document.createElement("span");
    likeIcon.innerHTML = '👍';
    likeIcon.className = "cursor-pointer text-lg hover:text-green-500";
    likeIcon.title = "Like";

    const dislikeIcon = document.createElement("span");
    dislikeIcon.innerHTML = '👎';
    dislikeIcon.className = "cursor-pointer text-lg hover:text-red-500";
    dislikeIcon.title = "Dislike";

    feedbackDiv.appendChild(likeIcon);
    feedbackDiv.appendChild(dislikeIcon);
    container.appendChild(feedbackDiv);

    // Add event listeners for like/dislike to the parent container (chatbox)
    feedbackDiv.addEventListener("click", function (event) {
        // Check if the clicked element is one of the icons
        if (event.target === likeIcon) {
            console.log("Like button clicked");
            rateResponse('like');
        } else if (event.target === dislikeIcon) {
            console.log("Dislike button clicked");
            rateResponse('dislike');
        }
    });
}

function rateResponse(type) {
    console.log(`Rate response: ${type} clicked`);

    // Append the feedback message to the chat UI
    const chatbox = document.getElementById("chatbox");
    const feedbackMessage = document.createElement("div");
    feedbackMessage.className = "p-4 max-w-xs rounded-lg text-sm bg-gray-200 text-left";
    feedbackMessage.textContent = type === 'like'
        ? "👍 Thanks for liking the response!"
        : "👎 Thanks for your feedback!";

    chatbox.appendChild(feedbackMessage);
    chatbox.scrollTop = chatbox.scrollHeight;

    // Send feedback to the backend (this should also work now)
    const lastQuestion = "Your last question here"; // You can pass the actual question from the user if needed
    fetch("http://127.0.0.1:8000/api/asdcfeedback/", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ feedback: type, query: lastUserQuery, collection_name: "chatbot_dgis" })
    }).then(res => res.json()).then(data => {
        console.log("Feedback stored:", data);
    }).catch(err => {
        console.warn("Failed to send feedback", err);
    });
}