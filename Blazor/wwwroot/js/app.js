var func = window.func || {};

func = {
    setStorage: function (name, value) {
        localStorage.setItem(name, value);
    },
    getStorage: function (name) {
        return localStorage.getItem(name);
    },
    switchTheme: function () {
        var currentTheme = this.getStorage('theme') || 'Light';
        var isDark = currentTheme === 'Dark';

        if (isDark) {
            document.querySelector('body').classList.add('dark-theme');
        } else {
            document.querySelector('body').classList.remove('dark-theme');
        }
    },
    clearAllStorage: function () {
        localStorage.clear();
        sessionStorage.clear();
    }
};

document.addEventListener('keydown', function (e) {
    if ((e.ctrlKey || e.metaKey) && e.key === 's') {
        e.preventDefault();
    }
});
