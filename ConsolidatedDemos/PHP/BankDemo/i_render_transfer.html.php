<div id="columnMain" style="margin-bottom:0 !important;padding-bottom:0 !important;">
    <div id="main-content" class="container">
        <p class="page-header">MAC Bank Funds Transfer</p>
        <div id="display_details" class="row">   
            <div  class="col-md-12 acct-label insert-detail-rows">
                <div id="errorMessage"  style="display: none;" class="alert alert-danger">
                    <?php if(isset($errorArray) && !empty($errorArray)) {
                            echo '<script type="text/javascript">$("#errorMessage").show()</script>';
                            $counter = 0;
                            foreach($errorArray as $msg) {
                                if($counter > 0)
                                    echo '<br />';
                                echo $msg;
                                $counter++;
                            }
                        }
                    ?>
                </div>

                <div id="successMessage"  style="display: none;" class="alert alert-success">
                    <?php if(isset($messageArray) && !empty($messageArray)) {
                                echo '<script type="text/javascript">$("#successMessage").show()</script>';
                        $counter = 0;
                        foreach($messageArray as $msg) {
                                if($counter > 0)
                                echo '<br />';
                                echo $msg;
                                $counter++;
                            }
                        }
                    ?>
                </div>
            </div>
        </div>
        <form id="transfer_funds" name="transfer_funds" method="POST" action="index.php?action=transferfunds" enctype="multipart/form-data">
            <div class="row">
                <?php    
                    if(!empty($myAccounts)) {
			
                        echo  '<div class="col-sm-6">
                            <label for="account_from">Transfer From Account</label> 
                            <select id="account_from" name="account_from" class="form-control">';
                            
                            if($selected_from_account == null) {    // if null then show 'select..' message
                            echo '<option value="" selected="selected">Select an Account</option>';
                    }
			        $selected = 'selected="selected"';
		            foreach($myAccounts as $account) {
                       $account = (array)$account;
			           $formattedNumber = substr($account['Number'], 0, 4).'-'.substr($account['Number'], 4, 4).'-'.substr($account['Number'], 8, 4).'-'.substr($account['Number'], 12, 4);  
			           $doSelected = '';
			           if($selected_from_account != '' && $selected_from_account == $account['Name'])
			              $doSelected = $selected;
			           echo "<option value='$account[Name]' $doSelected>$formattedNumber&nbsp;&nbsp;&nbsp;$account[Name] : $account[Balance]</option>";
		           }
                   echo '</select></div>';
                }           
	            ?>
                <div class="col-sm-6">
                    <label for="transfer_amount">Amount</label> 
                    <input name="transfer_amount" id="transfer_amount" type="text" class="form-control"  placeholder="Enter Amount to Tranfer" value="" />
                </div>
            </div><!-- end of row  -->

            <div class="row" style="margin-top: 15px;">
                <div class="col-sm-6">
                    <label for="account_to">Transfer To Account Holder</label> 
                    <input name="account_to_name" id="account_to_name" type="text" class="form-control"  placeholder="Enter Account Holder's Name" value="" />
                </div>
                <div class="col-sm-6">
                    <label for="transfer_amount">Transfer To Account Number</label> 
                    <input name="account_to_number" id="account_to_number" type="text" class="form-control"  placeholder="Enter Account Number" value="" />
                </div>
            </div><!-- end of row  -->  

            <div class="row" style="margin-top: 15px;">
                <div class="col-sm-12">
                    <input class="btn btn-warning" type="submit" value="Transfer" />
                </div>
            </div><!-- end of row  -->  

        </form>      
    </div>    
</div>
<!--Hide 3 column content area-->
<script type="text/javascript">
    $(document).ready(function () {
        $("#footer_txt").hide();
        $("#menu_transfers").addClass("selected");
    });
</script>
