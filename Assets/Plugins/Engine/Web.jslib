var web = {
    WebFree: function(pointer) {
	    _free(pointer);
    },

	WebMobile: function() {
		return /iPhone|iPad|iPod|Android/i.test(navigator.userAgent);
	},
};

mergeInto(LibraryManager.library, web);