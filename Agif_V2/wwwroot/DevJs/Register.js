function loadDropdown() {

    var Rank = $('#rank').data('rank-prefix');
    var UnitId = $('#UnitId').data('unitid-prefix');
    var regtCorps = $('#regtCorps').data('regtcorps-prefix');
    var ApptId = $('#ApptId').data('apptid-prefix');

    mMsater(Rank, "rank", 3, 0);
    mMsater(UnitId, "UnitId", 2, 0);
    mMsater(regtCorps, "regtCorps", 8, 0);
    mMsater(ApptId, "ApptId", 1, 0);
}



