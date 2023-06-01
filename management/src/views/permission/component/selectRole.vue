<template>
    <el-dialog v-model="dialogVisible" title="绑定角色" width="30%">
        <el-checkbox-group v-model="table.selectRoleData">
            <el-checkbox-button v-for="role in table.data" :key="role.id" :label="role.id">{{
                role.name
            }}</el-checkbox-button>
        </el-checkbox-group>
        <template #footer>
            <span class="dialog-footer">

                <el-button type="primary" @click="Save">
                    保存
                </el-button>
                <el-button @click="closeDialog">取消</el-button>
            </span>
        </template>
    </el-dialog>
</template>
  
<script lang="ts" setup>
import { ref } from 'vue'
import { ElMessage } from 'element-plus'
import api from '@/api/modules/role'
import userApi from '@/api/modules/user'

const table = reactive({
    data: [],
    selectRoleData: []
})

const props = defineProps({
    show: Boolean,
    userId: String
})
const dialogVisible = ref(false)
const emit = defineEmits(['update:show'])
watch(() => props.show, (val) => {
    console.log('父组件调用我拉')
    dialogVisible.value = val
    if (!dialogVisible.value) {
        table.selectRoleData = []
    } else {
        query().then(() => {
            userApi.GetUserBindRole(props.userId).then((res: any) => {
                table.selectRoleData = res
            })
        })
    }
})
watch(() => dialogVisible.value, (val) => {
    emit('update:show', val)
})

onMounted(async () => {
    // await query()
})


const query = async () => {
    let res = (await api.Query({ page: 1, pageSize: 9999 })) as any;
    table.data = res.data
}
const Save = async () => {
    console.log('userId', props.userId)
    console.log(table.selectRoleData)
    await userApi.SaveRole({ userId: props.userId, roleIds: table.selectRoleData })
    ElMessage.success('保存成功!')
    dialogVisible.value = false
}

const closeDialog = () => {
    dialogVisible.value = false
}
</script>
<style scoped>
.dialog-footer button:first-child {
    margin-right: 10px;
}
</style>