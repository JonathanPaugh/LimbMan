var web = {
    WebFree: function(pointer) {
	    _free(pointer);
    },
};

mergeInto(LibraryManager.library, web);